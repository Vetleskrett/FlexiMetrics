import type { RequestHandler } from '@sveltejs/kit';
import { api } from 'src/api.server';
import type { Readable } from 'stream';

export const GET: RequestHandler = async ({ params }) => {
  try {
    const response = await api.get(`/analyses/${params.analysisId}/status`, { responseType: 'stream' })

    const headers = new Headers();
    headers.set('Content-Type', response.headers['content-type'] || 'text/event-stream');
    headers.set('Cache-Control', 'no-cache');
    headers.set('Connection', 'keep-alive');

    const nodeStream = response.data as Readable;

    const webStream = new ReadableStream({
      start(controller) {
        nodeStream.on('data', (chunk) => controller.enqueue(chunk));
        nodeStream.on('end', () => controller.close());
        nodeStream.on('error', (err) => controller.error(err));
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
