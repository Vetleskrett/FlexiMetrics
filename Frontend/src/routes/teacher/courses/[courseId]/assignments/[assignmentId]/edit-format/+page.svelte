<script lang="ts">
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import type { Assignment, AssignmentField, Course } from 'src/types';
	import EditAssignmentFormatCard from 'src/components/EditAssignmentFormatCard.svelte';
	import { page } from '$app/stores';

	const courseId = $page.params.courseId;

	export let data: {
		course: Course;
		assignment: Assignment;
		assignmentFields: AssignmentField[];
	};
</script>

<div class="mx-auto mt-4 flex w-max flex-col gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses/{data.course.id}"
					>{data.course.code} - {data.course.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses/{data.course.id}/assignments/{data.assignment.id}"
					>{data.assignment.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>Edit format</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<EditAssignmentFormatCard
		{courseId}
		assignmentId={data.assignment.id}
		fields={data.assignmentFields}
	/>
</div>
