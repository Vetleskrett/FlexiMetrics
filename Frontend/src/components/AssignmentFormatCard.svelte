<script lang="ts">
	import type { AssignmentField } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';

	export let assignmentFields: AssignmentField[];
	export let courseId: string;
	export let assignmentId: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-4 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/metamorphose.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="ml-4 text-3xl">Format</Card.Title>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger class="ml-auto">
				<EllipsisVertical size="20" />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item href="/teacher/courses/{courseId}/assignments/{assignmentId}/edit-format">
					<Pencil class="h-4" />
					<p>Edit format</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</Card.Header>
	<Card.Content class="p-0">
		<Table.Root class="table-fixed">
			<Table.Header>
				<Table.Row>
					<Table.Head class="h-0 w-1/2 px-6 font-bold text-black">Name</Table.Head>
					<Table.Head class="h-0 px-6 font-bold text-black">Type</Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each assignmentFields as field}
					<Table.Row class="text-base">
						<Table.Cell class="px-6">
							<p>{field.name}</p>
						</Table.Cell>
						<Table.Cell class="px-6">
							{#if field.type == 'Range'}
								<p>Range ({field.rangeMin}-{field.rangeMax})</p>
							{:else}
								<p>{field.type}</p>
							{/if}
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
