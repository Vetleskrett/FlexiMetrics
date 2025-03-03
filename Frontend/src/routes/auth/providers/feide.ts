import type { Provider } from "@auth/sveltekit/providers";
import { setAuthToken } from "src/api";

export const FEIDE_PROVIDER: Provider = {
    id: 'feide',
    name: 'Feide',
    type: "oauth",
    options: {
        authorization : {
            params: {
                redirect_uri: "http://localhost:5173/auth/callback/feide",
            }
        },
    },
    clientId: "CLIENTID",
    clientSecret: "CLIENTSECRET",
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