import { toast } from 'svelte-sonner';

export function handleErrors<T>(action: () => Promise<T>) : Promise<T> {
    return handleFormErrors(undefined, action)
};

export async function handleFormErrors<T>(formErrors: any, action: () => Promise<T>) : Promise<T> {
    try {
        return await action();
    } catch (exception: any) {
        if (exception?.response?.data?.errors) {
            const errorArray = exception.response.data.errors;
            for (let error of errorArray) {
                toast.error(error.message);
            }

            if (formErrors) {
                const validationErrors = transformErrors(errorArray);
                formErrors.set(validationErrors);
            }
        } else {
            console.error(exception);
            toast.error('Error');
        }
        throw exception;
    }
};

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

export const cleanOptional = (value: any) => {
    if (value == '') {
        return undefined;
    }
    return value;
}