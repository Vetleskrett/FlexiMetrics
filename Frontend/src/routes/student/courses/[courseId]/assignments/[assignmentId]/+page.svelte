<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import AssignmentInformationCard from 'src/components/AssignmentInformationCard.svelte';
	import { onMount } from 'svelte';
	import { getCourse, getAssignment } from 'src/api';
	import type { Assignment, Course } from 'src/types';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	let course: Course;
	let assignment: Assignment;

	onMount(async () => {
		try {
			course = await getCourse(courseId);
			assignment = await getAssignment(assignmentId);
		} catch (error) {
			console.error('Something went wrong!');
		}
	});
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/student/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/student/courses/{courseId}"
					>{course?.code} - {course?.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{assignment?.name}</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/edit-text-file.png"
				alt="knowledge-sharing"
			/>
			<h1 class="ml-4 text-4xl font-semibold">{assignment?.name}</h1>
		</div>
	</div>
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8"></div>

		<div class="flex w-2/5 flex-col gap-8">
			<AssignmentInformationCard {assignment} />
		</div>
	</div>
</div>
