function generateColorFromString(str) {
    let hash = 0;
    const utf8 = new TextEncoder().encode(str);

    for (let i = 0; i < utf8.length; i++) {
        hash = ((hash << 5) - hash) + utf8[i];
        hash |= 0;
    }

    hash = Math.abs(hash);

    let color = '#';
    for (let i = 0; i < 3; i++) {
        const value = (hash >> (i * 8)) & 0xff;
        color += ('00' + value.toString(16)).substr(-2);
    }

    return color;
}

function hexToRgb(hex) {
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

function adjustBrightness(hex, amount) {
    let rgb = hexToRgb(hex);
    if (!rgb) return hex;

    rgb.r = Math.min(255, Math.max(0, rgb.r + amount));
    rgb.g = Math.min(255, Math.max(0, rgb.g + amount));
    rgb.b = Math.min(255, Math.max(0, rgb.b + amount));

    return `rgb(${rgb.r}, ${rgb.g}, ${rgb.b})`;
}