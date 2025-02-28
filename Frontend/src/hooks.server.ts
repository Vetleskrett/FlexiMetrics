import { redirect, type Handle } from '@sveltejs/kit';
import { handle as authenticationHandle } from './routes/auth/route';
import { sequence } from '@sveltejs/kit/hooks';
import { hasToken } from './api';
 
const authorizationHandle: Handle = async ({ event, resolve }) => {
  if (event.url.pathname.startsWith('/')) {
    const session = await event.locals.auth();
    if (!session || !hasToken()) {
      // Redirect to the signin page
      throw redirect(303, '/auth/signin');
    }
  }
 
  // If the request is still here, just proceed as normally
  return resolve(event);
}

 
// First handle authentication, then authorization
// Each function acts as a middleware, receiving the request handle
// And returning a handle which gets passed to the next function
export const handle: Handle = sequence(authenticationHandle, authorizationHandle)