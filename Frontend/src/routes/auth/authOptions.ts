import type { SvelteKitAuthConfig } from "@auth/sveltekit/";
import { FEIDE_PROVIDER } from "./providers/feide";

export const authOptions : SvelteKitAuthConfig = {
    providers: [FEIDE_PROVIDER],
    callbacks: {
        async session(params: any) {
            if(params.token){
                //const JWT = (token.token as { token: {sub: string}}).token
                return {...params};
            }
            return params.session
        },
        async jwt({ token }) {
            return token
        }
    },
    secret: "ANYSECRET",
    session:{
        strategy: 'jwt',
    },
}