import type { RequestHandler } from '@sveltejs/kit';
import { api } from 'src/api.server';
import type { Readable } from 'stream';

export const GET: RequestHandler = async ({ params }) => {
  try {
    const response = await api.get(`/analyzers/${params.analyzerId}/status`, { responseType: 'stream' })

    if (response.status < 200 || response.status >= 300) {
      const status = response?.status || 500;
      const message = response?.statusText || 'Internal Server Error';
      return new Response(message, { status });
    }

    const headers = new Headers();
    headers.set('Content-Type', response.headers['content-type'] || 'text/event-stream');
    headers.set('Cache-Control', 'no-cache');
    headers.set('Connection', 'keep-alive');

    const nodeStream = response.data as Readable;

    const webStream = new ReadableStream({
      start(controller) {
        nodeStream.on('data', (chunk) => {
          try {
            controller.enqueue(chunk);
          } catch(error: any) {
            console.error(error);
          }
        });
        nodeStream.on('end', () => {
          try {
            controller.close();
          } catch(error: any) {
            console.error(error);
          }
        });
        nodeStream.on('error', (err) => {
          try {
            controller.error(err);
          } catch(error: any) {
            console.error(error);
          }
        });
      }
    });

    return new Response(webStream, {
      status: 200,
      headers,
    });
  } catch (error: any) {
    const status = error?.response?.status || 500;
    const message = error?.response?.statusText || 'Internal Server Error';
    return new Response(message, { status });
  }
};
