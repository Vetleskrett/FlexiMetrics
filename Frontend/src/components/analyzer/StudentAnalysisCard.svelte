<script lang="ts">
	import type { StudentAnalysis } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import { Separator } from '$lib/components/ui/separator';
	import { getCell } from './cells/cells';
	import { Render } from 'svelte-headless-table';

	export let studentAnalysis: StudentAnalysis;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<Card.Title class="m-0 ml-2 text-2xl">{studentAnalysis.analyzerName}</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#each studentAnalysis.fields as field}
				{@const renderer = getCell(field.type, field.subType)}
				<Separator class="w-full" />
				<div class="px-6 py-4">
					<h1 class="font-semibold">
						{field.name}
					</h1>
					<Render of={renderer({ value: field })} />
				</div>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
