import type { SvelteKitAuthConfig } from "@auth/sveltekit/";
import { FEIDE_PROVIDER } from "./providers/feide";

export const authOptions : SvelteKitAuthConfig = {
    providers: [FEIDE_PROVIDER],
    pages: {
        signIn: '/sign-in'
    },
    callbacks: {
        async session(params: any) {
            if(params.token){
                return {...params};
            }
            return params.session
        },
        async jwt({ token }) {
            return token
        }
    },
    secret: import.meta.env.VITE_AUTH_SECRET,
    session:{
        strategy: 'jwt'
    },
}