<script lang="ts">
	import type { Assignment, AssignmentField, CreateDelivery, UpdateDelivery, Delivery } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Checkbox } from 'src/lib/components/ui/checkbox';
	import { Input } from 'src/lib/components/ui/input';
	import Textarea from 'src/lib/components/ui/textarea/textarea.svelte';
	import Save from 'lucide-svelte/icons/save';
	import CustomButton from './CustomButton.svelte';
	import { studentId } from 'src/store';
	import { postDelivery, putDelivery, postDeliveryFieldFile } from 'src/api';
	import { goto } from '$app/navigation';
	import Undo_2 from 'lucide-svelte/icons/undo-2';
	import FileUpload from './FileUpload.svelte';

	export let assignment: Assignment;
	export let assignmentFields: AssignmentField[];
	export let delivery: Delivery | null;

	type FileField = {
		assignmentFieldId: string;
		file: File;
	};

	let values = Object.fromEntries(
		assignmentFields.map((assignmentField) => {
			const field = delivery
					? delivery.fields.find((x) => x.assignmentFieldId == assignmentField.id)?.value
					: undefined;
			return [
				assignmentField.id,
				field
			]
		})
	);

	const onSubmitEdit = async () => {
		const request: UpdateDelivery = {
			fields: assignmentFields.map((assignmentField) => {
				let value = values[assignmentField.id];

				if (assignmentField.type == 'Integer' || assignmentField.type == 'Float') {
					value = Number(value);
				}

				if (assignmentField.type == 'File' && value instanceof File) {
					value = {
						FileName: value.name,
						ContentType: value.type
					}
				}

				return {
					assignmentFieldId: assignmentField.id,
					value: value
				};
			})
		};
		const response = await putDelivery(delivery?.id || '', request);
		delivery = response.data;
	}

	const onSubmitCreate = async () => {
		const request: CreateDelivery = {
			assignmentId: assignment.id,
			studentId: studentId,
			fields: assignmentFields.map((assignmentField) => {
				let value = values[assignmentField.id];

				if (assignmentField.type == 'Integer' || assignmentField.type == 'Float') {
					value = Number(value);
				}

				if (assignmentField.type == 'File' && value instanceof File) {
					value = {
						FileName: value.name,
						ContentType: value.type
					}
				}

				return {
					assignmentFieldId: assignmentField.id,
					value: value
				};
			})
		};
		const response = await postDelivery(request);
		delivery = response.data;
	};

	const onSubmit = async () => {
		const fileFields: FileField[] = [];

		for(let assignmentField of assignmentFields) {
			const value = values[assignmentField.id];
			if (assignmentField.type == 'File' && value instanceof File) {
				fileFields.push({
					assignmentFieldId: assignmentField.id,
					file: value
				});
			}
		}

		if (delivery == null) {
			await onSubmitCreate();
		} else {
			await onSubmitEdit();
		}

		for(let fileField of fileFields) {
			const deliveryField = delivery!.fields.find(f => f.assignmentFieldId == fileField.assignmentFieldId);
			if (deliveryField) {
				await postDeliveryFieldFile(deliveryField.id, fileField.file);
			}
		}

		goto('./');
	}

</script>

<Card.Root class="w-[800px] overflow-hidden p-0">
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
							<FileUpload bind:file={values[assignmentField.id]}/>
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
