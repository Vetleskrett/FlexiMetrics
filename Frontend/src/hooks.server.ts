import { redirect, type Handle } from '@sveltejs/kit';
import { handle as authenticationHandle } from './routes/auth/route';
import { sequence } from '@sveltejs/kit/hooks';
 
const authorizationHandle: Handle = async ({ event, resolve }) => {
  const path = event.url.pathname

  if (path.startsWith('/teacher') || path.startsWith('/student')) {
    const session = await event.locals.auth();

    if (!session) {
      throw redirect(303, '/auth/signin?callbackUrl=' + path);
    }
  }
 
  return resolve(event);
}

 
// First handle authentication, then authorization
// Each function acts as a middleware, receiving the request handle
// And returning a handle which gets passed to the next function
export const handle: Handle = sequence(authenticationHandle, authorizationHandle)