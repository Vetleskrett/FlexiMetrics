import { authOptions } from "./authOptions";
import { SvelteKitAuth } from "@auth/sveltekit";

export const {handle, signIn, signOut } = SvelteKitAuth(authOptions);