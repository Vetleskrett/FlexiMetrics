import type { RequestHandler } from '@sveltejs/kit';
import { Readable } from 'stream';
import { api } from 'src/api.server';

export const GET: RequestHandler = async ({ params }) => {
  try {
    const fileResponse = await api.get(`analysis-fields/${params.analysisFieldId}`, { responseType: 'stream' });

    if (fileResponse.status < 200 || fileResponse.status >= 300) {
      const status = fileResponse?.status || 500;
      const message = fileResponse?.statusText || 'Internal Server Error';
      return new Response(message, { status });
    }

    const headers = new Headers();
    headers.set('Content-Disposition', fileResponse.headers['content-disposition'] || 'attachment');
    headers.set('Content-Type', fileResponse.headers['content-type'] || 'application/octet-stream');
    if (fileResponse.headers['content-length']) {
      headers.set('Content-Length', fileResponse.headers['content-length']);
    }

    const nodeStream = fileResponse.data as Readable;

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
