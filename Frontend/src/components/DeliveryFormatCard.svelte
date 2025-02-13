<script lang="ts">
	import type { AssignmentField, RegisterAssignmentField } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import Ellipsis from 'lucide-svelte/icons/ellipsis';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import Plus from 'lucide-svelte/icons/plus';
	import CustomButton from './CustomButton.svelte';
	import Separator from 'src/lib/components/ui/separator/separator.svelte';
	import Undo_2 from 'lucide-svelte/icons/undo-2';
	import Save from 'lucide-svelte/icons/save';
	import { addAssignmentFields, deleteAssigmentField } from 'src/api';
	import { Input } from 'src/lib/components/ui/input';
	import * as Select from '$lib/components/ui/select/index';

	type NewAssignmentField = {
		name: string;
		type: {
			label: string;
			value: string;
		};
	};

	let newFields: NewAssignmentField[] = [];

	function addField() {
		newFields.push({
			name: '',
			type: {
				label: 'String',
				value: 'String'
			}
		});
		newFields = newFields;
	}

	function removeField(field: NewAssignmentField) {
		const index = newFields.indexOf(field, 0);
		if (index > -1) {
			newFields.splice(index, 1);
			newFields = newFields;
		}
	}

	function deleteFieldFromList(field: AssignmentField) {
		const index = assignmentFields.indexOf(field, 0);
		if (index > -1) {
			assignmentFields.splice(index, 1);
			assignmentFields = assignmentFields;
		}
	}

	function removeAllFields() {
		newFields = [];
	}

	async function addNewFields() {
		try {
			const response = await addAssignmentFields({
				fields: newFields.map((field) => {
					return {
						name: field.name,
						type: field.type.value,
						assignmentId: assignmentId
					};
				})
			});
			assignmentFields = assignmentFields.concat(response.data);
			newFields = [];
		} catch (exception) {
			console.error('Something Went Wrong!');
		}
	}

	async function deleteField(assignmentField: AssignmentField) {
		try {
			await deleteAssigmentField(assignmentField.id);
			deleteFieldFromList(assignmentField);
		} catch (exception) {
			console.error('Something Went Wrong!');
		}
	}

	export let assignmentFields: AssignmentField[];
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
			<Card.Title class="ml-4 text-3xl">Delivery Format</Card.Title>
		</div>

		<CustomButton on:click={addField} color="green">
			<Plus />
			<p>New</p>
		</CustomButton>
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
									<DropdownMenu.Item on:click={() => deleteField(assignmentField)}>
										<Trash2 class="h-4" />
										<p>Delete field</p>
									</DropdownMenu.Item>
								</DropdownMenu.Content>
							</DropdownMenu.Root>
						</Table.Cell>
					</Table.Row>
				{/each}
				{#each newFields as assignmentField}
					<Table.Row>
						<Table.Cell>
							<Input bind:value={assignmentField.name} />
						</Table.Cell>
						<Table.Cell>
							<Select.Root bind:selected={assignmentField.type}>
								<Select.Trigger>
									<Select.Value />
								</Select.Trigger>
								<Select.Content>
									<Select.Item value="String" label="String">String</Select.Item>
									<Select.Item value="Integer" label="Integer">Integer</Select.Item>
									<Select.Item value="Double" label="Double">Double</Select.Item>
									<Select.Item value="Boolean" label="Boolean">Boolean</Select.Item>
									<Select.Item value="File" label="File">File</Select.Item>
								</Select.Content>
							</Select.Root>
						</Table.Cell>
						<Table.Cell class="flex justify-end px-6">
							<CustomButton
								color="red"
								outline={true}
								on:click={() => removeField(assignmentField)}
							>
								<Trash_2 size="16" />
							</CustomButton>
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
		{#if newFields.length > 0}
			<Card.Footer class="items-middle flex w-full flex-col p-0">
				<Separator />
				<div class="m-4 flex gap-4">
					<CustomButton color="yellow" on:click={removeAllFields}>
						<Undo_2 size="20" />
						<p>Remove new fields</p>
					</CustomButton>
					<CustomButton color="submit" on:click={addNewFields}>
						<Save size="20" />
						<p>Save</p>
					</CustomButton>
				</div>
			</Card.Footer>
		{/if}
	</Card.Content>
</Card.Root>
