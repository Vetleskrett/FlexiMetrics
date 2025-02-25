// custom-auth.d.ts

// Import existing types from @auth/core/types
import type { Session } from '@auth/core/types';

// Extend the Session type to include custom properties
declare module '@auth/core/types' {
    interface Session {
        user?:{
            name?: string | null
            email?: string | null
            image?: string | null
            sub: string
        }
        test?: string | undefined;
        abc?: any | undefined;
    }
}
