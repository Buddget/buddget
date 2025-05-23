﻿using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Enums;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Buddget.BLL.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFinancialSpaceRepository _financialSpaceRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;

        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository,              ICategoryRepository categoryRepository,      IUserRepository userRepository, IFinancialSpaceRepository financialSpaceRepository, IMapper mapper, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;

            _financialSpaceRepository = financialSpaceRepository;
            _mapper = mapper;
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        public async Task<string> CreateTransactionAsync(TransactionDto transactionDto)
        {
            try
            {
                // Перевірка фінансового простору
                var financialSpace = await _financialSpaceRepository.GetFinancialSpaceAsync(transactionDto.FinancialSpaceId);
                if (financialSpace == null)
                {
                    _logger.LogWarning("Financial space with ID {SpaceId} was not found when creating transaction.", transactionDto.FinancialSpaceId);
                    return "Financial space not found.";
                }

                // Перевірка користувача
                var user = await _userRepository.GetByIdAsync(transactionDto.AuthorId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} was not found when creating transaction.", transactionDto.AuthorId);
                    return "User not found.";
                }

                // Перевірка категорії, якщо вказана
                CategoryEntity category = null;
                if (transactionDto.CategoryId.HasValue)
                {
                    category = await _categoryRepository.GetByIdAsync(transactionDto.CategoryId.Value);
                    if (category == null)
                    {
                        _logger.LogWarning("Category with ID {CategoryId} was not found when creating transaction.", transactionDto.CategoryId);
                        return "Category not found.";
                    }
                }

                // Валідація суми
                if (transactionDto.Amount <= 0)
                {
                    _logger.LogWarning("Invalid amount {Amount} when creating transaction.", transactionDto.Amount);
                    return "Transaction amount must be greater than zero.";
                }

                // Валідація типу транзакції
                if (string.IsNullOrEmpty(transactionDto.Type) || 
                    (transactionDto.Type != "Income" && transactionDto.Type != "Expense"))
                {
                    _logger.LogWarning("Invalid transaction type {Type} when creating transaction.", transactionDto.Type);
                    return "Transaction type must be either 'Income' or 'Expense'.";
                }

                // Створення об'єкту транзакції
                var transaction = new TransactionEntity
                {
                    Name = transactionDto.Name,
                    Amount = transactionDto.Amount,
                    Currency = transactionDto.Currency ?? "UAH",
                    Date = transactionDto.Date,
                    Description = transactionDto.Description,
                    Type = transactionDto.Type,
                    CategoryId = transactionDto.CategoryId,
                    UserId = transactionDto.AuthorId,
                    FinancialSpaceId = transactionDto.FinancialSpaceId,
                };

                // Збереження транзакції
                await _transactionRepository.AddAsync(transaction);
                _logger.LogInformation("Transaction created successfully. ID: {TransactionId}, Space: {SpaceId}, User: {UserId}",
                    transaction.Id, transaction.FinancialSpaceId, transaction.UserId);

                return "Transaction created successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction in space {SpaceId} by user {UserId}",
                    transactionDto.FinancialSpaceId, transactionDto.AuthorId);
                return $"An error occurred while creating the transaction: {ex.Message}";
            }
        }
        public async Task<IEnumerable<TransactionDto>> GetTransactionsBySpaceIdAsync(int spaceId)
        {
            var transactions = await _transactionRepository.GetTransactionsBySpaceIdAsync(spaceId);

            var transactionDtos = new List<TransactionDto>();

            foreach (var transaction in transactions)
            {
                var dto = new TransactionDto
                {
                    Id = transaction.Id,
                    Name = transaction.Name,
                    Amount = transaction.Amount,
                    Currency = transaction.Currency,
                    Date = transaction.Date,
                    Description = transaction.Description ?? string.Empty,
                    CategoryName = transaction.Category?.Name ?? string.Empty,
                    AuthorName = transaction.User != null
                        ? $"{transaction.User.FirstName} {transaction.User.LastName}"
                        : "Unknown",
                    Type = transaction.Type,
                };

                transactionDtos.Add(dto);
            }

            return transactionDtos;
        }

        public async Task<string> DeleteTransactionAsync(int transactionId, int userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction with ID {TransactionId} was not found.", transactionId);
                return "Transaction not found.";
            }

            var financialSpace = await _financialSpaceRepository.GetFinancialSpaceAsync(transaction.FinancialSpaceId);
            if (financialSpace == null)
            {
                _logger.LogWarning("Financial space with ID {SpaceId} was not found.", transaction.FinancialSpaceId);
                return "Financial space not found.";
            }

            if (transaction.UserId != userId && financialSpace.OwnerId != userId)
            {
                _logger.LogWarning("User with ID {UserId} is not authorized to delete transaction with ID {TransactionId}.", userId, transactionId);
                return "You are not authorized to delete this transaction.";
            }

            try
            {
                await _transactionRepository.DeleteAsync(transaction);
                _logger.LogInformation("Successfully deleted transaction with ID {TransactionId}.", transactionId);
                return "Transaction deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to delete transaction with ID {TransactionId}.", transactionId);
                return "An error occurred while deleting the transaction.";
            }
        }

        public async Task<IEnumerable<TransactionDto>> GetSortedTransactionsBySpaceIdAsync(int spaceId, TransactionSortColumnEnum sortColumn = TransactionSortColumnEnum.Id, bool ascending = true)
        {
            var transactions = await GetTransactionsBySpaceIdAsync(spaceId);
            var sortedTransactions = SortTransactions(transactions, sortColumn, ascending);
            _logger.LogInformation($"Retrieved {sortedTransactions.Count()} transactions for space with ID {spaceId}. Sorted by {sortColumn} in {(ascending ? "ascending" : "descending")} order.");
            return _mapper.Map<IEnumerable<TransactionDto>>(sortedTransactions);
        }

        public async Task<string> MoveTransactionAsync(int transactionId, int targetSpaceId, int userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction not found");
                return "Transaction not found.";
            }

            var currentSpace = await _financialSpaceRepository.GetFinancialSpaceAsync(transaction.FinancialSpaceId);
            var targetSpace = await _financialSpaceRepository.GetFinancialSpaceAsync(targetSpaceId);

            if (currentSpace == null || targetSpace == null)
            {
                _logger.LogWarning("One or both of the financial spaces were not found.");
                return "One or both of the financial spaces were not found.";
            }

            if (currentSpace.OwnerId != userId && (currentSpace.Members == null || !currentSpace.Members.Any(m => m.UserId == userId)))
            {
                _logger.LogWarning("User is not authorized to move the transaction from this space.");
                return "You are not authorized to move the transaction from this space.";
            }

            if (targetSpace.OwnerId != userId && (targetSpace.Members == null || !targetSpace.Members.Any(m => m.UserId == userId)))
            {
                _logger.LogWarning("User is not authorized to move the transaction to the target space.");
                return "You are not authorized to move the transaction to the target space.";
            }

            transaction.FinancialSpaceId = targetSpaceId;
            await _transactionRepository.UpdateAsync(transaction);

            _logger.LogInformation("Transaction moved successfully.");
            return "Transaction moved successfully.";
        }

        private IEnumerable<TransactionDto> SortTransactions(IEnumerable<TransactionDto> transactions, TransactionSortColumnEnum sortColumn, bool ascending = true)
        {
            return sortColumn switch
            {
                TransactionSortColumnEnum.Id => ascending ? transactions.OrderBy(t => t.Id) : transactions.OrderByDescending(t => t.Id),
                TransactionSortColumnEnum.Name => ascending ? transactions.OrderBy(t => t.Name) : transactions.OrderByDescending(t => t.Name),
                TransactionSortColumnEnum.Amount => ascending ? transactions.OrderBy(t => t.Amount) : transactions.OrderByDescending(t => t.Amount),
                TransactionSortColumnEnum.Currency => ascending ? transactions.OrderBy(t => t.Currency) : transactions.OrderByDescending(t => t.Currency),
                TransactionSortColumnEnum.Date => ascending ? transactions.OrderBy(t => t.Date) : transactions.OrderByDescending(t => t.Date),
                TransactionSortColumnEnum.Type => ascending ? transactions.OrderBy(t => t.Type) : transactions.OrderByDescending(t => t.Type),
                TransactionSortColumnEnum.CategoryName => ascending ? transactions.OrderBy(t => t.CategoryName) : transactions.OrderByDescending(t => t.CategoryName),
                TransactionSortColumnEnum.AuthorName => ascending ? transactions.OrderBy(t => t.AuthorName) : transactions.OrderByDescending(t => t.AuthorName),
                TransactionSortColumnEnum.Description => ascending ? transactions.OrderBy(t => t.Description) : transactions.OrderByDescending(t => t.Description),
                _ => transactions
            };
        }
    }
}
