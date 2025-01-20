<script lang="ts">
	import type { Assignment } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';

	export let assignment: Assignment;

	$: rows = [
		{
			label: 'Due',
			value: assignment?.dueDate.split("T")[0]
		},
		{
			label: 'Collaberation Type',
			value: assignment?.collaborationType
		},
		{
			label: 'Published',
			value: assignment?.published ? 'Yes' : 'No'
		}
	];
	console.log(rows)
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="40"
				height="40"
				src="https://img.icons8.com/fluency/480/info.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-2 text-2xl">Information</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#each rows as row}
				<Separator class="w-full" />
				<div class="flex items-center justify-between px-6 py-4">
					<h1 class="font-semibold">{row.label}</h1>
					<h1>{row.value}</h1>
				</div>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
