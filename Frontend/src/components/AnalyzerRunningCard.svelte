<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import { Progress } from '$lib/components/ui/progress';
	import { ScrollArea } from '$lib/components/ui/scroll-area/index.js';

	const completed = 32;
	const total = 49;

	const logs = `
2024-09-26 14:05:58 Trying again in 6.00 seconds... (3/100)
2024-09-26 14:05:58 
2024-09-26 14:06:04 [2024-09-26 12:06:04,407: INFO/MainProcess] Connected to amqp://guest:**@rabbitmq:5672//
2024-09-26 14:06:04 [2024-09-26 12:06:04,408: WARNING/MainProcess] /usr/local/lib/python3.11/site-packages/celery/worker/consumer/consumer.py:507: CPendingDeprecationWarning: The broker_connection_retry configuration setting will no longer determine
2024-09-26 14:06:04 whether broker connection retries are made during startup in Celery 6.0 and above.
2024-09-26 14:06:04 If you wish to retain the existing behavior for retrying connections on startup,
2024-09-26 14:06:04 you should set broker_connection_retry_on_startup to True.
2024-09-26 14:06:04   warnings.warn(
2024-09-26 14:06:04 
2024-09-26 14:06:04 [2024-09-26 12:06:04,413: INFO/MainProcess] mingle: searching for neighbors
2024-09-26 14:06:05 [2024-09-26 12:06:05,441: INFO/MainProcess] mingle: all alone
2024-09-26 14:06:05 [2024-09-26 12:06:05,469: INFO/MainProcess] celery@aca4762c9f33 ready.
2024-09-26 14:05:58 Trying again in 6.00 seconds... (3/100)
2024-09-26 14:05:58 
2024-09-26 14:06:04 [2024-09-26 12:06:04,407: INFO/MainProcess] Connected to amqp://guest:**@rabbitmq:5672//
2024-09-26 14:06:04 [2024-09-26 12:06:04,408: WARNING/MainProcess] /usr/local/lib/python3.11/site-packages/celery/worker/consumer/consumer.py:507: CPendingDeprecationWarning: The broker_connection_retry configuration setting will no longer determine
2024-09-26 14:06:04 whether broker connection retries are made during startup in Celery 6.0 and above.
2024-09-26 14:06:04 If you wish to retain the existing behavior for retrying connections on startup,
2024-09-26 14:06:04 you should set broker_connection_retry_on_startup to True.
2024-09-26 14:06:04   warnings.warn(
2024-09-26 14:06:04 
2024-09-26 14:06:04 [2024-09-26 12:06:04,413: INFO/MainProcess] mingle: searching for neighbors
2024-09-26 14:06:05 [2024-09-26 12:06:05,441: INFO/MainProcess] mingle: all alone
2024-09-26 14:06:05 [2024-09-26 12:06:05,469: INFO/MainProcess] celery@aca4762c9f33 ready.
`.split('\n');
</script>

<Card.Root class="w-[1080px]">
	<Card.Content class="flex items-center gap-2 p-2">
		<div class="mx-4 flex w-96 flex-col items-center gap-2 px-4">
			<h2 class="text-sm font-semibold text-gray-500">Teams completed</h2>
			<h1 class="text-5xl font-semibold">{completed} / {total}</h1>
			<Progress value={completed} max={total} class="mt-2 h-4" />
		</div>
		<ScrollArea class="h-64 rounded-lg bg-background text-xs">
			<div class="px-4 py-2">
				{#each logs as log}
					<p>{log}</p>
				{/each}
			</div>
		</ScrollArea>
	</Card.Content>
</Card.Root>
