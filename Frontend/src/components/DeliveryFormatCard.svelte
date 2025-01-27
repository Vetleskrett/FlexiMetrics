<script lang="ts">
	import type { AssignmentField } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import Ellipsis from 'lucide-svelte/icons/ellipsis';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import Plus from 'lucide-svelte/icons/plus';
	import CustomButton from './CustomButton.svelte';

	export let assignmentFields: AssignmentField[];
	export let assignmentId: string;
	export let courseId: string;
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
			<Card.Title class="ml-4 text-3xl">Delivery Format</Card.Title>
		</div>

		<CustomButton
			href="/courses/{courseId}/assignments/{assignmentId}/delivery-format"
			color="green"
		>
			<Plus />
			<p>New</p>
		</CustomButton>
	</Card.Header>
	<Card.Content class="p-0">
		<Table.Root>
			<Table.Header>
				<Table.Row>
					<Table.Head class="h-0 px-6 font-bold text-black">Name</Table.Head>
					<Table.Head class="h-0 px-6 font-bold text-black">Type</Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each assignmentFields as assignmentField}
					<Table.Row class="text-base">
						<Table.Cell class="px-6">
							{assignmentField.name}
						</Table.Cell>
						<Table.Cell class="px-6">
							{assignmentField.type}
						</Table.Cell>
						<Table.Cell class="flex justify-end px-6">
							<DropdownMenu.Root>
								<DropdownMenu.Trigger class="ml-auto">
									<Ellipsis size="20" />
								</DropdownMenu.Trigger>
								<DropdownMenu.Content>
									<DropdownMenu.Item>
										<Trash2 class="h-4" />
										<p>Delete field</p>
									</DropdownMenu.Item>
								</DropdownMenu.Content>
							</DropdownMenu.Root>
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
