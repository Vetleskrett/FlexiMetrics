<script lang="ts">
	import { page } from '$app/stores';
	import type { Course, Student } from 'src/types/';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import AllStudentsCard from 'src/components/student/AllStudentsCard.svelte';
	import AddStudentsCard from 'src/components/student/AddStudentsCard.svelte';
	import { postStudentsCourse } from 'src/api';

	const courseId = $page.params.courseId;

	export let data: {
		course: Course;
		students: Student[];
	};

	async function addStudents(input: string, file: File | null) {
		if (file) {
			input = await file.text();
		}
		const studentEmails = handleInput(input);
		try {
			if (studentEmails) {
				var response = await postStudentsCourse(courseId, {
					emails: studentEmails
				});
				data.students = response.data;
			}
		} catch (error) {
			console.error('Something went wrong!');
		}
	}

	function handleInput(studentInput: string) {
		const input = studentInput.split(',');
		for (const student of input) {
			if (!checkStudent(student)) {
				console.warn('Something is wrong with the input');
				return;
			}
		}
		return input;
	}

	function checkStudent(email: string): boolean {
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
				<Breadcrumb.Page>Students</Breadcrumb.Page>
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
				<h1 class="ml-4 text-4xl font-semibold">Students</h1>
			</div>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				<EllipsisVertical size={32} />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item>
					<Trash2 class="h-4" />
					<p>Remove All Students</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</div>
	<div class="flex flex-row gap-8">
		<div class="flex w-[700px] flex-col gap-8">
			<AllStudentsCard students={data.students} {courseId} />
		</div>
		<div class="flex w-[400px] flex-col gap-8">
			<AddStudentsCard addFunction={addStudents} />
		</div>
	</div>
</div>
