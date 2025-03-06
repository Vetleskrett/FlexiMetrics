<script lang="ts">
	import type {
		Assignment,
		AssignmentField,
		CreateDelivery,
		UpdateDelivery,
		Delivery
	} from 'src/types/';
	import * as Card from '$lib/components/ui/card/index.js';
	import Save from 'lucide-svelte/icons/save';
	import CustomButton from '../CustomButton.svelte';
	import { studentId } from 'src/store';
	import { goto } from '$app/navigation';
	import Undo_2 from 'lucide-svelte/icons/undo-2';
	import DeliveryFieldInput from '../inputs/DeliveryFieldInput.svelte';
	import { postDelivery, postDeliveryFieldFile, putDelivery } from 'src/api';

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
			return [assignmentField.id, field];
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
					};
				}

				return {
					assignmentFieldId: assignmentField.id,
					value: value
				};
			})
		};
		const response = await putDelivery(delivery!.id, request);
		delivery = response.data;
	};

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
					};
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

		for (let assignmentField of assignmentFields) {
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

		for (let fileField of fileFields) {
			const deliveryField = delivery!.fields.find(
				(f) => f.assignmentFieldId == fileField.assignmentFieldId
			);
			if (deliveryField) {
				var formData = new FormData();
				formData.append('file', fileField.file);
				await postDeliveryFieldFile(deliveryField.id, formData);
			}
		}
		goto('./');
	};
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
							<span class="text-gray-500">
								{#if assignmentField.type == 'ShortText' || assignmentField.type == 'LongText'}
									(Text)
								{:else if assignmentField.type == 'List'}
									{#if assignmentField.subType == 'ShortText' || assignmentField.subType == 'LongText'}
										(List of Text)
									{:else}
										(List of {assignmentField.subType}s)
									{/if}
								{:else}
									({assignmentField.type})
								{/if}
							</span>
						</label>

						<DeliveryFieldInput
							bind:value={values[assignmentField.id]}
							type={assignmentField.type}
							subType={assignmentField.subType}
						/>
					</div>
				{/each}
				<div class="mb-8 mt-4 flex w-full justify-center gap-4">
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
