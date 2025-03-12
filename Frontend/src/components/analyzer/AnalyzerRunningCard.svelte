<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Progress } from '$lib/components/ui/progress';
	import { ScrollArea } from '$lib/components/ui/scroll-area';
	import type { Analysis } from 'src/types';
	import { onMount } from 'svelte';

	export let analysis: Analysis;

	const total = analysis.totalNumEntries;
	const completed = analysis.analysisEntries.length;

	const scrollToBottom = () => {
		const scrollArea = document.querySelector('[data-scroll-area-viewport]');
		if (scrollArea) {
			scrollArea.scrollTop = scrollArea.scrollHeight;
		}
	};

	onMount(() => {
		setTimeout(scrollToBottom, 10);
	});
</script>

<Card.Root class="w-[1080px]">
	<Card.Content class="flex items-center gap-2 p-2">
		<div class="mx-4 flex w-96 flex-col items-center gap-2 px-4">
			<h2 class="text-sm font-semibold text-gray-500">Completed</h2>
			<h1 class="text-5xl font-semibold">{completed} / {total}</h1>
			<Progress value={completed} max={total} class="mt-2 h-4" />
		</div>
		<ScrollArea class="h-64 w-full rounded-lg bg-background text-xs">
			<div class="flex flex-col gap-2 px-4 py-2">
				{#each analysis.analysisEntries as entry}
					<div>
						{#if entry.student}
							<p class="m-0 p-0 font-bold">[{entry.student.name}]</p>
						{:else}
							<p class="m-0 p-0 font-bold">[Team {entry.team?.teamNr}]</p>
						{/if}
						{#if entry.logInformation}
							<p style="white-space: pre-wrap" class="m-0 p-0">{entry.logInformation}</p>
						{/if}
						{#if entry.logError}
							<p style="white-space: pre-wrap" class="m-0 p-0 text-red-500">{entry.logError}</p>
						{/if}
					</div>
				{/each}
			</div>
		</ScrollArea>
	</Card.Content>
</Card.Root>
