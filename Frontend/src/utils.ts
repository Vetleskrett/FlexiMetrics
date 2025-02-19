export function transformErrors(errors: { propertyName: string; message: string }[]) {
    const result: Record<string, any> = {};

    for (const error of errors) {
        const path = error.propertyName
            .toLowerCase()
            .replace(/\[(\d+)\]/g, '.$1') // Convert array-like paths
            .split('.');

        let current = result;
        while (path.length > 1) {
            const key = path.shift()!;
            current[key] = current[key] || (isNaN(Number(path[0])) ? {} : []);
            current = current[key];
        }

        const finalKey = path.shift()!;
        if (!current[finalKey]) {
            current[finalKey] = [];
        }
        current[finalKey].push(error.message);
    }

    return result;
}