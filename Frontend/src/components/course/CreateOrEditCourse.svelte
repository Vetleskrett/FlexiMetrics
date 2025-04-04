<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Semester, type Course } from 'src/types/';
	import CustomButton from '../CustomButton.svelte';
	import Save from 'lucide-svelte/icons/save';
	import Undo_2 from 'lucide-svelte/icons/undo-2';
	import { Input } from '$lib/components/ui/input';
	import * as RadioGroup from '$lib/components/ui/radio-group';
	import { goto } from '$app/navigation';
	import { teacherId } from 'src/store';
	import * as Form from 'src/lib/components/ui/form';
	import { superForm } from 'sveltekit-superforms';
	import { postCourse, putCourse } from 'src/api';
	import { handleFormErrors } from 'src/utils';

	export let edit: boolean;
	export let course: Course = {
		id: '',
		name: '',
		code: '',
		year: new Date().getFullYear(),
		semester: Semester.Spring
	};

	const onSubmitEdit = async () => {
		return putCourse(course.id, {
			name: course.name,
			code: course.code,
			year: Number(course.year),
			semester: course.semester.toString()
		});
	};

	const onSubmitCreate = async () => {
		return postCourse({
			name: course.name,
			code: course.code,
			year: Number(course.year),
			semester: course.semester.toString(),
			teacherId: teacherId
		});
	};

	const onSubmit = async (formEvent: any) => {
		formEvent.cancel();

		await handleFormErrors(errors, async () => {
			const promise = edit ? onSubmitEdit() : onSubmitCreate();
			const response = await promise;
			var course = response.data;
			goto(`/teacher/courses/${course.id}`);
		});
	};

	const form = superForm(course, {
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
				<Card.Title class="ml-4 text-3xl">{edit ? 'Edit Course' : 'New Course'}</Card.Title>
			</div>
		</Card.Header>
		<Card.Content class="flex flex-col gap-4 px-6 py-0">
			<Form.Field {form} name="name">
				<Form.Control let:attrs>
					<Form.Label for="name">Name</Form.Label>
					<Input {...attrs} id="name" bind:value={course.name} />
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>
			<Form.Field {form} name="code">
				<Form.Control let:attrs>
					<Form.Label for="code">Code</Form.Label>
					<Input {...attrs} id="code" bind:value={course.code} />
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>
			<Form.Field {form} name="year">
				<Form.Control let:attrs>
					<Form.Label for="year">Year</Form.Label>
					<Input
						{...attrs}
						id="year"
						type="number"
						min="2000"
						max="2099"
						bind:value={course.year}
					/>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>
			<Form.Field {form} name="semester">
				<Form.Control let:attrs>
					<Form.Label for="semester">Semester</Form.Label>
					<RadioGroup.Root {...attrs} id="semester" bind:value={course.semester}>
						<div class="flex items-center gap-1">
							<RadioGroup.Item {...attrs} id="spring" value={Semester.Spring} />
							<label for="spring">Spring</label>
						</div>
						<div class="flex items-center gap-1">
							<RadioGroup.Item {...attrs} id="autumn" value={Semester.Autumn} />
							<label for="autumn">Autumn</label>
						</div>
					</RadioGroup.Root>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>
		</Card.Content>
		<Card.Footer class="flex flex-col items-center p-0">
			<div class="m-4 flex gap-4">
				<CustomButton color="yellow" href="./">
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
