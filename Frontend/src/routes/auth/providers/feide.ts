import type { Provider } from "@auth/sveltekit/providers";
import { setAuthToken } from "src/api.server";

export const FEIDE_PROVIDER: Provider = {
    id: 'feide',
    name: 'Feide',
    type: "oauth",
    options: {
        authorization : {
            params: {
                redirect_uri: import.meta.env.MODE == 'production' ?
                    "http://localhost:4173/auth/callback/feide" :
                    "http://localhost:5173/auth/callback/feide",
            }
        },
    },
    clientId: import.meta.env.VITE_FEIDE_CLIENT_ID,
    clientSecret: import.meta.env.VITE_FEIDE_CLIENT_SECRET,
    token: 'https://auth.dataporten.no/oauth/token',
    authorization: 'https://auth.dataporten.no/oauth/authorization',
    userinfo: 'https://auth.dataporten.no/openid/userinfo',
    wellKnown: 'https://auth.dataporten.no/.well-known/openid-configuration',
    issuer:"https://auth.dataporten.no",
    style:{
        logo: "https://www.feide.no/sites/default/files/Logo.png",
    },
    async profile(profile, _tokens) {
        setAuthToken(_tokens.id_token!)
        return { ...profile, id: profile }
    }
}