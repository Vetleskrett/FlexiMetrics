<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import CreateOrEditAnalyzer from 'src/components/CreateOrEditAnalyzer.svelte';
	import type { Analyzer, Assignment, Course } from 'src/types';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;
	const analyzerId = $page.params.analyzerId;

	export let data: {
		course: Course;
		assignment: Assignment;
		analyzer: Analyzer;
		script: string;
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
				<Breadcrumb.Link href="/teacher/courses/{courseId}"
					>{data.course.code} - {data.course.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Link href="/teacher/courses/{courseId}/assignments/{assignmentId}">
				{data.assignment.name}
			</Breadcrumb.Link>
			<Breadcrumb.Separator />
			<Breadcrumb.Link
				href="/teacher/courses/{courseId}/assignments/{assignmentId}/analyzers/{analyzerId}"
			>
				{data.analyzer.name}
			</Breadcrumb.Link>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>Edit</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<CreateOrEditAnalyzer
		{courseId}
		{assignmentId}
		analyzer={data.analyzer}
		script={data.script}
		edit={true}
	/>
</div>
