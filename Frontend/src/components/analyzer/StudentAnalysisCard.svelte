<script lang="ts">
	import type { StudentAnalysis } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import { Separator } from '$lib/components/ui/separator';
	import StringCell from './cells/StringCell.svelte';
	import BoolCell from './cells/BoolCell.svelte';
	import RangeCell from './cells/RangeCell.svelte';
	import DateCell from './cells/DateCell.svelte';
	import UrlCell from './cells/UrlCell.svelte';
	import JsonCell from './cells/JsonCell.svelte';

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
				<Separator class="w-full" />
				<div class="px-6 py-4">
					<h1 class="font-semibold">
						{field.name}
					</h1>
					{#if field.type == 'Boolean'}
						<BoolCell value={field.value} />
					{:else if field.type == 'Range'}
						<RangeCell value={field.value} />
					{:else if field.type == 'DateTime'}
						<DateCell value={field.value} />
					{:else if field.type == 'URL'}
						<UrlCell value={field.value} />
					{:else if field.type == 'Json'}
						<JsonCell value={field.value} />
					{:else}
						<StringCell value={field.value} />
					{/if}
				</div>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
