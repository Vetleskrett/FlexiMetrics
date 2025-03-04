<script lang="ts">
	import { page } from '$app/stores';
	import type { Course, Student } from 'src/types';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import axios from 'axios';
	import SimpleAddCard from 'src/components/SimpleAddCard.svelte';
	import AllTeachersCard from 'src/components/AllTeachersCard.svelte';

	const courseId = $page.params.courseId;

	export let data: {
		course: Course;
		teachers: Student[];
	};

	async function addTeacher(input: string) {
		const teacherEmail = checkTeacher(input);
		try {
			if (teacherEmail) {
				var response = await axios.post(`/api/course/${courseId}/teachers`, { email: input.trim() });
				data.teachers = response.data;
			}
		} catch (error) {
			console.error('Something went wrong!');
		}
	}


	function checkTeacher(email: string): boolean {
		// Add additional checks if needed
		if (email.trim().length < 1) {
			return false;
		}
		return true;
	}
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses/{courseId}"
					>{data.course.code} - {data.course.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>Teachers</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/student-male.png"
				alt="group"
			/>
			<div>
				<h1 class="ml-4 text-4xl font-semibold">Teachers</h1>
			</div>
		</div>
	</div>
	<div class="flex flex-row gap-8">
		<div class="flex w-[700px] flex-col gap-8">
			<AllTeachersCard teachers={data.teachers} {courseId} />
		</div>
		<div class="flex w-[400px] flex-col gap-8">
			<SimpleAddCard 
			headline="Add Teacher"
			actionString="Add"
			inputString="Email"
			inputType="String"
			addFunction={addTeacher}/>
		</div>
	</div>
</div>
