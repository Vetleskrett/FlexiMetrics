<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import type {
		Assignment,
		CollaborationType,
		Course,
		GradingType,
		AssignmentField,
		AssignmentFieldType
	} from 'src/types';
	import CustomButton from './CustomButton.svelte';
	import Save from 'lucide-svelte/icons/save';
	import Undo_2 from 'lucide-svelte/icons/undo-2';
	import { Checkbox } from 'src/lib/components/ui/checkbox';
	import { Input } from '$lib/components/ui/input';
	import { Textarea } from '$lib/components/ui/textarea';
	import * as RadioGroup from '$lib/components/ui/radio-group';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import Plus from 'lucide-svelte/icons/plus';
	import { editAssignment, postAssignment } from 'src/api';
	import { goto } from '$app/navigation';
	import * as Form from 'src/lib/components/ui/form';
	import { superForm } from 'sveltekit-superforms';
	import { CalendarDate } from '@internationalized/date';
	import DatePicker from './DatePicker.svelte';
	import * as Select from '$lib/components/ui/select/index';

	const today = new Date();

	export let edit: boolean;
	export let course: Course;
	export let assignment: Assignment | undefined = undefined;
	export let fields: AssignmentField[] | undefined = undefined;

	type AssignmentFieldFormData = {
		name: string;
		type: {
			label: AssignmentFieldType;
			value: AssignmentFieldType;
		};
	};

	type AssignmentFormData = {
		name: string;
		dueDate: CalendarDate;
		collaborationType: CollaborationType;
		mandatory: boolean;
		gradingType: GradingType;
		maxPoints?: number;
		description: string;
		fields: AssignmentFieldFormData[];
	};

	let assignmentFormData: AssignmentFormData = {
		name: '',
		dueDate: new CalendarDate(today.getFullYear(), today.getMonth() + 1, today.getDate()),
		collaborationType: 'Individual',
		mandatory: true,
		gradingType: 'NoGrading',
		maxPoints: undefined,
		description: '',
		fields: [
			{
				name: '',
				type: {
					label: 'String',
					value: 'String'
				}
			}
		]
	};

	if (edit) {
		const date = new Date(Date.parse(assignment!.dueDate));
		assignmentFormData = {
			name: assignment!.name,
			dueDate: new CalendarDate(date.getFullYear(), date.getMonth() + 1, date.getDate()),
			collaborationType: assignment!.collaborationType,
			mandatory: assignment!.mandatory,
			gradingType: assignment!.gradingType,
			maxPoints: assignment!.maxPoints,
			description: assignment!.description,
			fields: fields!.map((field) => {
				return {
					name: field.name,
					type: {
						label: field.type,
						value: field.type
					}
				};
			})
		};
	}

	function addField() {
		assignmentFormData.fields.push({
			name: '',
			type: {
				label: 'String',
				value: 'String'
			}
		});
		assignmentFormData.fields = assignmentFormData.fields;
	}

	function removeField(field: AssignmentFieldFormData) {
		const index = assignmentFormData.fields.indexOf(field, 0);
		if (index > -1) {
			assignmentFormData.fields.splice(index, 1);
			assignmentFormData.fields = assignmentFormData.fields;
		}
	}

	const onSubmitEdit = async () => {
		return editAssignment(assignment!.id, {
			name: assignmentFormData.name,
			description: assignmentFormData.description,
			dueDate: assignmentFormData.dueDate.toDate('utc').toJSON(),
			gradingType: assignmentFormData.gradingType,
			maxPoints: assignmentFormData?.maxPoints,
			mandatory: assignmentFormData.mandatory,
			published: assignment!.published,
			collaborationType: assignmentFormData.collaborationType
		});
	};

	const onSubmitCreate = async () => {
		return postAssignment({
			name: assignmentFormData.name,
			description: assignmentFormData.description,
			courseId: course.id,
			dueDate: assignmentFormData.dueDate.toDate('utc').toJSON(),
			gradingType: assignmentFormData.gradingType,
			maxPoints: assignmentFormData?.maxPoints,
			mandatory: assignmentFormData.mandatory,
			published: false,
			collaborationType: assignmentFormData.collaborationType,
			fields: assignmentFormData.fields.map((fieldFormData) => {
				return {
					name: fieldFormData.name,
					type: fieldFormData.type.value
				};
			})
		});
	};

	function transformErrors(errors: { propertyName: string; message: string }[]) {
		const result: Record<string, any> = {};

		for (const error of errors) {
			const path = error.propertyName
				.toLowerCase()
				.replace(/\[(\d+)\]/g, '.$1') // Convert array-like paths
				.split('.');

			let current = result;
			while (path.length > 1) {
				const key = path.shift()!;
				current[key] = current[key] || (isNaN(Number(path[0])) ? {} : []);
				current = current[key];
			}

			const finalKey = path.shift()!;
			if (!current[finalKey]) {
				current[finalKey] = [];
			}
			current[finalKey].push(error.message);
		}

		return result;
	}

	const onSubmit = async (formEvent: any) => {
		formEvent.cancel();
		var promise = edit ? onSubmitEdit() : onSubmitCreate();

		promise
			.then((response) => {
				var assignment = response.data;
				goto(`/teacher/courses/${course.id}/assignments/${assignment.id}`);
			})
			.catch((exception) => {
				const validationErrors = transformErrors(exception.response.data.errors);
				console.log(validationErrors);
				errors.set(validationErrors);
			});
	};

	const form = superForm(assignmentFormData, {
		onSubmit: onSubmit,
		dataType: 'json'
	});
	const { enhance, errors } = form;
</script>

<form method="post" use:enhance>
	<Card.Root class="m-auto w-[700px] overflow-hidden p-0">
		<Card.Header class="mb-6 flex flex-row items-center justify-between">
			<div class="flex items-center">
				<img
					width="48"
					height="48"
					src="https://img.icons8.com/fluency/480/knowledge-sharing.png"
					alt="knowledge-sharing"
				/>
				<Card.Title class="ml-4 text-3xl">{edit ? 'Edit Assignment' : 'New Assignment'}</Card.Title>
			</div>
		</Card.Header>
		<Card.Content class="flex flex-col gap-4 px-6 py-0">
			<Form.Field {form} name="name">
				<Form.Control let:attrs>
					<Form.Label for="name">Name</Form.Label>
					<Input {...attrs} id="name" bind:value={assignmentFormData.name} />
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			<Form.Field {form} name="description">
				<Form.Control let:attrs>
					<Form.Label for="description">Description</Form.Label>
					<Textarea {...attrs} id="description" bind:value={assignmentFormData.description} />
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			<Form.Field {form} name="dueDate" class="flex flex-col">
				<Form.Control let:attrs>
					<Form.Label for="dueDate">Due Date</Form.Label>
					<DatePicker {...attrs} id="dueDate" bind:value={assignmentFormData.dueDate} />
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			<Form.Field {form} name="mandatory" class="flex flex-col">
				<Form.Control let:attrs>
					<Form.Label for="mandatory">Mandatory</Form.Label>
					<Checkbox {...attrs} id="mandatory" bind:checked={assignmentFormData.mandatory} />
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			<Form.Field {form} name="collaborationType" class="flex flex-col">
				<Form.Control let:attrs>
					<Form.Label for="collaborationType">Collaboration</Form.Label>
					<RadioGroup.Root
						{...attrs}
						id="collaborationType"
						bind:value={assignmentFormData.collaborationType}
					>
						<div class="flex items-center gap-1">
							<RadioGroup.Item {...attrs} id="individual" value="Individual" />
							<label for="spring">Individual</label>
						</div>
						<div class="flex items-center gap-1">
							<RadioGroup.Item {...attrs} id="teams" value="Teams" />
							<label for="autumn">Teams</label>
						</div>
					</RadioGroup.Root>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			<Form.Field {form} name="gradingType" class="flex flex-col">
				<Form.Control let:attrs>
					<Form.Label for="gradingType">Grading</Form.Label>
					<RadioGroup.Root {...attrs} id="gradingType" bind:value={assignmentFormData.gradingType}>
						<div class="flex items-center gap-1">
							<RadioGroup.Item {...attrs} id="noGrading" value="NoGrading" />
							<label for="spring">None</label>
						</div>
						<div class="flex items-center gap-1">
							<RadioGroup.Item {...attrs} id="approvalGrading" value="ApprovalGrading" />
							<label for="autumn">Approval</label>
						</div>
						<div class="flex items-center gap-1">
							<RadioGroup.Item {...attrs} id="letterGrading" value="LetterGrading" />
							<label for="spring">Letter</label>
						</div>
						<div class="flex items-center gap-1">
							<RadioGroup.Item {...attrs} id="pointsGrading" value="PointsGrading" />
							<label for="spring">Points</label>
						</div>
					</RadioGroup.Root>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			{#if assignmentFormData.gradingType == 'PointsGrading'}
				<Form.Field {form} name="maxPoints">
					<Form.Control let:attrs>
						<Form.Label for="maxPoints">Max Points</Form.Label>
						<Input
							{...attrs}
							type="number"
							id="maxPoints"
							bind:value={assignmentFormData.maxPoints}
						/>
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			{/if}

			{#if !edit}
				<div class="mt-4 flex flex-col gap-2">
					<h1 class="text-lg font-medium">Fields</h1>
					{#each assignmentFormData.fields as field, i}
						<div class="flex w-full gap-2">
							<Form.Field {form} name="fields[{i}].name" class="w-full">
								<Form.Control let:attrs>
									<Form.Label for="fields[{i}].name">Name</Form.Label>
									<Input
										{...attrs}
										id="fields[{i}].name"
										bind:value={assignmentFormData.fields[i].name}
									/>
								</Form.Control>
								<Form.FieldErrors />
							</Form.Field>

							<Form.Field {form} name="fields[{i}].type" class="w-full">
								<Form.Control let:attrs>
									<Form.Label for="fields[{i}].type">Type</Form.Label>
									<Select.Root {...attrs} bind:selected={assignmentFormData.fields[i].type}>
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
								</Form.Control>
								<Form.FieldErrors />
							</Form.Field>

							<div class="mt-8">
								<CustomButton color="red" outline={true} on:click={() => removeField(field)}>
									<Trash_2 size="16" on:click={() => removeField(field)} />
								</CustomButton>
							</div>
						</div>
					{/each}
					<div class="flex items-center justify-center">
						<CustomButton color={'green'} outline={true} on:click={addField}>
							<Plus size="16" />
						</CustomButton>
					</div>
				</div>
			{/if}
		</Card.Content>
		<Card.Footer class="items-middle mt-8 flex flex-col p-0">
			<div class="m-4 flex gap-4">
				<CustomButton color="yellow" href={edit ? './' : '../'}>
					<Undo_2 size="20" />
					<p>Return</p>
				</CustomButton>
				<CustomButton color="submit">
					<Save size="20" />
					<p>{edit ? 'Edit' : 'Create'}</p>
				</CustomButton>
			</div>
		</Card.Footer>
	</Card.Root>
</form>
