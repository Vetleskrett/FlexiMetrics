import type { SvelteKitAuthConfig } from "@auth/sveltekit/";
import { FEIDE_PROVIDER } from "./providers/feide";

export const authOptions : SvelteKitAuthConfig  = {
    providers: [FEIDE_PROVIDER], 
    callbacks: {
        async session({ token, session }) {
            if(token){
                //const JWT = (token.token as { token: {sub: string}}).token
                return{
                    ...session,
                    user: {
                        ...session.user,
                        sub: token.sub,
                        
                    }, 
                    test: token
                    
                };
            }
            return session
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