<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import { Progress } from '$lib/components/ui/progress';
	import { ScrollArea } from '$lib/components/ui/scroll-area/index.js';
	import type { Analysis } from 'src/types';

	export let analysis: Analysis;
	export let status: {
		total: number;
		completed: number;
		logs: string;
	};
</script>

{#if analysis?.status == 'Started' || analysis?.status == 'Running'}
	<Card.Root class="w-[1080px]">
		<Card.Content class="flex items-center gap-2 p-2">
			<div class="mx-4 flex w-96 flex-col items-center gap-2 px-4">
				<h2 class="text-sm font-semibold text-gray-500">Completed</h2>
				<h1 class="text-5xl font-semibold">{status.completed} / {status.total}</h1>
				<Progress value={status.completed} max={status.total} class="mt-2 h-4" />
			</div>
			<ScrollArea class="h-64 w-full rounded-lg bg-background text-xs">
				<div class="px-4 py-2">
					<p style="white-space: pre-wrap">{status.logs}</p>
				</div>
			</ScrollArea>
		</Card.Content>
	</Card.Root>
{/if}
