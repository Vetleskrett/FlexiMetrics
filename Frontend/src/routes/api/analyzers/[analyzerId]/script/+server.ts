import { getAnalyzerScript, postAnalyzerScript } from 'src/api';
import { json, type RequestHandler } from '@sveltejs/kit';
import { Readable } from 'stream';

export const POST: RequestHandler = async ({ params, request }) => {
  const payload = await request.formData()
  const response = await postAnalyzerScript(params.analyzerId as string, payload)
  return json(response.data)
}

export const GET: RequestHandler = async ({ params }) => {
  try {
    const fileResponse = await getAnalyzerScript(params.analyzerId as string);

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
