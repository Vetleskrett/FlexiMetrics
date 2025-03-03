<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import type { AssignmentField, AssignmentFieldFormData } from 'src/types';
	import CustomButton from './CustomButton.svelte';
	import Save from 'lucide-svelte/icons/save';
	import Undo_2 from 'lucide-svelte/icons/undo-2';
	import { superForm } from 'sveltekit-superforms';
	import AssignmentFieldsForm from './AssignmentFieldsForm.svelte';
	import { cleanOptional, transformErrors } from 'src/utils';
	import { goto } from '$app/navigation';
	import axios from 'axios';

	export let courseId: string;
	export let assignmentId: string;
	export let fields: AssignmentField[];

	let formData: {
		fields: AssignmentFieldFormData[];
	} = {
		fields: fields.map((f) => {
			return {
				...f,
				type: {
					label: f.type,
					value: f.type
				},
				subType: f.subType
					? {
							label: f.subType,
							value: f.subType
						}
					: undefined
			};
		})
	};

	const onSubmit = async (formEvent: any) => {
		formEvent.cancel();
		console.log(formData);
		axios.put(`/api/assignments/fields/${assignmentId}`, {
			fields: formData.fields.map((fieldFormData) => {
				return {
					...fieldFormData,
					type: fieldFormData.type.value,
					min: cleanOptional(fieldFormData.min),
					max: cleanOptional(fieldFormData.max),
					regex: cleanOptional(fieldFormData.regex),
					subType: cleanOptional(fieldFormData.subType?.value)
				};
			})
		})
			.then((response: any) => {
				fields = response.data;
				goto(`/teacher/courses/${courseId}/assignments/${assignmentId}`);
			})
			.catch((exception) => {
				if (exception?.response?.data?.errors) {
					const validationErrors = transformErrors(exception.response.data.errors);
					console.error(validationErrors);
					errors.set(validationErrors);
				} else {
					console.error(exception);
				}
			});
	};

	const form = superForm(formData, {
		onSubmit: onSubmit,
		dataType: 'json'
	});
	const { enhance, errors } = form;
</script>

<form method="post" use:enhance>
	<Card.Root class="m-auto w-[700px] overflow-hidden p-0">
		<Card.Header class="flex flex-row items-center justify-between">
			<div class="flex items-center">
				<img
					width="48"
					height="48"
					src="https://img.icons8.com/fluency/480/metamorphose.png"
					alt="metamorphose"
				/>
				<Card.Title class="ml-4 text-3xl">Edit Format</Card.Title>
			</div>
		</Card.Header>
		<Card.Content class="flex flex-col gap-4 p-0">
			<AssignmentFieldsForm {form} bind:fields={formData.fields} />
		</Card.Content>
		<Card.Footer class="items-middle mt-8 flex flex-col p-0">
			<div class="m-4 flex gap-4">
				<CustomButton color="yellow" href="./">
					<Undo_2 size="20" />
					<p>Return</p>
				</CustomButton>
				<CustomButton color="submit">
					<Save size="20" />
					<p>Save</p>
				</CustomButton>
			</div>
		</Card.Footer>
	</Card.Root>
</form>
