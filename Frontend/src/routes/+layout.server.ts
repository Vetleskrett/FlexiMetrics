import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async (event) => {
    const session = await event.locals.auth();
    const loggedIn = session != null;

	return {
		loggedIn
	};
};