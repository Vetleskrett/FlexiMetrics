<script lang="ts">
	import type { Assignment, AssignmentField, CreateDelivery, Delivery, Team } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Checkbox } from 'src/lib/components/ui/checkbox';
	import { Input } from 'src/lib/components/ui/input';
	import Textarea from 'src/lib/components/ui/textarea/textarea.svelte';
	import Save from 'lucide-svelte/icons/save';
	import CustomButton from './CustomButton.svelte';
	import X from 'lucide-svelte/icons/x';
	import { studentId } from 'src/store';
	import { postDelivery } from 'src/api';
	import { goto } from '$app/navigation';
	import Undo_2 from 'lucide-svelte/icons/undo-2';

	export let assignment: Assignment;
	export let assignmentFields: AssignmentField[];
	export let delivery: Delivery | null;

	let values = Object.fromEntries(
		assignmentFields.map((assignmentField) => [
			assignmentField.id,
			delivery
				? delivery.fields.find((x) => x.assignmentFieldId == assignmentField.id)?.value
				: undefined
		])
	);

	const onSubmit = async () => {
		console.log(values);
		const delivery: CreateDelivery = {
			assignmentId: assignment.id,
			studentId: studentId,
			fields: assignmentFields.map((assignmentField) => {
				return {
					assignmentFieldId: assignmentField.id,
					value:
						assignmentField.type == 'Integer' || assignmentField.type == 'Float'
							? Number(values[assignmentField.id])
							: values[assignmentField.id]
				};
			})
		};
		await postDelivery(delivery);
		goto('./');
	};
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/edit-text-file.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-4 text-3xl">{assignment.name}</Card.Title>
		</div>
		<p class="font-semibold text-gray-500">
			Due {new Date(assignment?.dueDate).toLocaleDateString()}
		</p>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			<form method="POST" on:submit|preventDefault={onSubmit}>
				{#each assignmentFields as assignmentField}
					<div class="flex flex-col gap-1 px-6 py-4">
						<label for={assignmentField.name}>
							<span class="font-semibold">{assignmentField.name}</span>
							<span class="text-gray-500">({assignmentField.type.replace(/([A-Z])/g, ' $1').substring(1)})</span>
						</label>

						{#if assignmentField.type == 'ShortText'}
							<Input bind:value={values[assignmentField.id]} />
						{:else if assignmentField.type == 'LongText'}
							<Textarea class="h-[200px]" bind:value={values[assignmentField.id]} />
						{:else if assignmentField.type == 'Integer'}
							<Input type="number" bind:value={values[assignmentField.id]} />
						{:else if assignmentField.type == 'Float'}
							<Input type="number" step="any" bind:value={values[assignmentField.id]} />
						{:else if assignmentField.type == 'Boolean'}
							<Checkbox bind:checked={values[assignmentField.id]} />
						{:else if assignmentField.type == 'URL'}
							<Input bind:value={values[assignmentField.id]} />
						{:else if assignmentField.type == 'File'}
							<Input type="file" />
						{/if}
					</div>
				{/each}
				<div class="mt-4 mb-8 flex justify-center w-full gap-4">
					<CustomButton color="yellow" href="./">
						<Undo_2 size="20" />
						<p>Return</p>
					</CustomButton>
					<CustomButton color="submit">
						<Save size="20" />
						<p>Submit</p>
					</CustomButton>
				</div>
			</form>
		</div>
	</Card.Content>
</Card.Root>
