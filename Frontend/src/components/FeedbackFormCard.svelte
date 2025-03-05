<script lang="ts">
	import type { Assignment, Feedback, LetterGrade } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Input } from 'src/lib/components/ui/input';
	import Textarea from 'src/lib/components/ui/textarea/textarea.svelte';
	import Save from 'lucide-svelte/icons/save';
	import CustomButton from './CustomButton.svelte';
	import * as Form from 'src/lib/components/ui/form';
	import { superForm } from 'sveltekit-superforms';
	import * as RadioGroup from '$lib/components/ui/radio-group';
	import * as Select from '$lib/components/ui/select/index';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import X from 'lucide-svelte/icons/x';
	import Check from 'lucide-svelte/icons/check';
	import axios from 'axios';
	import { postFeedback, putFeedback } from 'src/api';

	export let assignment: Assignment;
	export let feedback: Feedback | null;
	export let studentId: string | undefined = undefined;
	export let teamId: string | undefined = undefined;

	let dueDatePassed = Date.parse(assignment.dueDate) < new Date().getTime();
	let edit = dueDatePassed && feedback == null;

	type FeedbackFormData = {
		comment: string;
		assignmentId: string;
		studentId?: string;
		teamId?: string;
		isApproved?: string;
		letterGrade?: {
			label: LetterGrade;
			value: LetterGrade;
		};
		points?: number;
	};

	const toFormData: () => FeedbackFormData = () => {
		return {
			comment: feedback?.comment ?? '',
			assignmentId: assignment?.id,
			teamId: teamId,
			studentId: studentId,
			isApproved:
				feedback?.isApproved == undefined
					? undefined
					: feedback!.isApproved
						? 'Approved'
						: 'Disapproved',
			letterGrade:
				feedback?.letterGrade == undefined
					? undefined
					: {
							label: feedback?.letterGrade,
							value: feedback?.letterGrade
						},
			points: feedback?.points ?? undefined
		};
	};

	let feedbackFormData: FeedbackFormData = toFormData();

	const onSubmitEdit = async () => {
		return putFeedback(feedback!.id, {
			comment: feedbackFormData.comment,
			isApproved:
				feedbackFormData.isApproved == undefined
					? undefined
					: feedbackFormData.isApproved == 'Approved',
			letterGrade: feedbackFormData.letterGrade?.value,
			points: feedbackFormData.points
		});
	};

	const onSubmitCreate = async () => {
		return postFeedback({
			comment: feedbackFormData.comment,
			assignmentId: assignment.id,
			studentId: studentId,
			teamId: teamId,
			isApproved:
				feedbackFormData.isApproved == undefined
					? undefined
					: feedbackFormData.isApproved == 'Approved',
			letterGrade: feedbackFormData.letterGrade?.value,
			points: feedbackFormData.points
		});
	};

	const onCancel = () => {
		edit = false;
		feedbackFormData = toFormData();
	};

	const onSubmit = async (formEvent: any) => {
		formEvent.cancel();
		var promise = feedback != null ? onSubmitEdit() : onSubmitCreate();

		promise
			.then((response) => {
				feedback = response.data;
				edit = false;
			})
			.catch((exception) => {
				const fieldErrors = exception?.response?.data?.errors?.filter(
					(error: any) => error?.propertyName != undefined
				);
				if (fieldErrors) {
					const validationErrors = Object.fromEntries(
						fieldErrors.map((error: any) => [error.propertyName.toLowerCase(), [error.message]])
					);
					errors.set(validationErrors);
				}

				const otherErrors = exception?.response?.data?.errors?.filter(
					(error: any) => error?.propertyName == undefined
				);
				if (otherErrors) {
					for (let error of otherErrors) {
						console.error(error?.message);
					}
				}
			});
	};

	const form = superForm(feedbackFormData, {
		onSubmit: onSubmit,
		dataType: 'json'
	});
	const { enhance, errors } = form;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/popular-topic--v1.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-4 text-3xl">Feedback</Card.Title>
		</div>
		{#if dueDatePassed && !edit}
			<DropdownMenu.Root>
				<DropdownMenu.Trigger class="ml-auto">
					<EllipsisVertical size="20" />
				</DropdownMenu.Trigger>
				<DropdownMenu.Content>
					<DropdownMenu.Item on:click={() => (edit = true)}>
						<Pencil class="h-4" />
						<p>Edit feedback</p>
					</DropdownMenu.Item>
				</DropdownMenu.Content>
			</DropdownMenu.Root>
		{/if}
	</Card.Header>
	{#if edit}
		<form method="POST" use:enhance>
			<Card.Content class="flex flex-col gap-4 p-6">
				{#if assignment.gradingType == 'ApprovalGrading'}
					<Form.Field {form} name="isApproved">
						<Form.Control let:attrs>
							<Form.Label for="isApproved">Grading</Form.Label>
							<RadioGroup.Root {...attrs} id="isApproved" bind:value={feedbackFormData.isApproved}>
								<div class="flex items-center gap-1">
									<RadioGroup.Item {...attrs} id="approved" value="Approved" />
									<label for="spring">Approved</label>
								</div>
								<div class="flex items-center gap-1">
									<RadioGroup.Item {...attrs} id="disapproved" value="Disapproved" />
									<label for="autumn">Disapproved</label>
								</div>
							</RadioGroup.Root>
						</Form.Control>
						<Form.FieldErrors />
					</Form.Field>
				{:else if assignment.gradingType == 'LetterGrading'}
					<Form.Field {form} name="letterGrade">
						<Form.Control let:attrs>
							<Form.Label for="letterGrade">Grading</Form.Label>
							<Select.Root {...attrs} bind:selected={feedbackFormData.letterGrade}>
								<Select.Trigger>
									<Select.Value />
								</Select.Trigger>
								<Select.Content>
									<Select.Item value="A" label="A">A</Select.Item>
									<Select.Item value="B" label="B">B</Select.Item>
									<Select.Item value="C" label="C">C</Select.Item>
									<Select.Item value="D" label="D">D</Select.Item>
									<Select.Item value="E" label="E">E</Select.Item>
									<Select.Item value="F" label="F">F</Select.Item>
								</Select.Content>
							</Select.Root>
						</Form.Control>
						<Form.FieldErrors />
					</Form.Field>
				{:else if assignment.gradingType == 'PointsGrading'}
					<Form.Field {form} name="points">
						<Form.Control let:attrs>
							<Form.Label for="points">Grading (0-{assignment.maxPoints})</Form.Label>
							<Input
								type="number"
								{...attrs}
								min="0"
								max={assignment.maxPoints}
								id="points"
								bind:value={feedbackFormData.points}
							/>
						</Form.Control>
						<Form.FieldErrors />
					</Form.Field>
				{/if}

				<Form.Field {form} name="comment">
					<Form.Control let:attrs>
						<Form.Label for="comment">Comment</Form.Label>
						<Textarea
							{...attrs}
							class="h-[200px]"
							id="comment"
							bind:value={feedbackFormData.comment}
						/>
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
				<div class="flex justify-end gap-4">
					{#if feedback != null}
						<CustomButton color="red" on:click={onCancel}>
							<X size="20" />
							<p>Cancel</p>
						</CustomButton>
					{/if}

					<CustomButton color="submit">
						<Save size="20" />
						<p>Save</p>
					</CustomButton>
				</div>
			</Card.Content>
		</form>
	{:else if !dueDatePassed}
		<Card.Content class="flex justify-center p-6">
			<p>Cannot give feedback before due date has passed</p>
		</Card.Content>
	{:else}
		<Card.Content class="flex flex-col gap-4 p-6">
			{#if assignment?.gradingType != 'NoGrading'}
				<div>
					<h1 class="font-semibold">Grading</h1>
					{#if assignment.gradingType == 'ApprovalGrading'}
						<div class="flex h-full items-center justify-start gap-1">
							{#if feedback?.isApproved}
								<Check color="green" />
								<p>Approved</p>
							{:else}
								<X color="red" />
								<p>Disapproved</p>
							{/if}
						</div>
					{:else if assignment.gradingType == 'LetterGrading'}
						<p>{feedback?.letterGrade}</p>
					{:else if assignment.gradingType == 'PointsGrading'}
						<p>{feedback?.points} / {assignment?.maxPoints}</p>
					{/if}
				</div>
			{/if}
			<div>
				<h1 class="font-semibold">Comment</h1>
				<h1>{feedback?.comment}</h1>
			</div>
		</Card.Content>
	{/if}
</Card.Root>
