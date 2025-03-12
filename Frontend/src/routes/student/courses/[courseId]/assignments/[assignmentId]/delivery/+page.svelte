<script lang="ts">
	import { page } from '$app/stores';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb';
	import DeliveryFormCard from 'src/components/delivery/DeliveryFormCard.svelte';
	import type { Assignment, AssignmentField, Course, Delivery } from 'src/types/';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	export let data: {
		course: Course;
		assignment: Assignment;
		assignmentFields: AssignmentField[];
		delivery: Delivery | null;
	};
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/student/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/student/courses/{courseId}">
					{data.course.code} - {data.course.name}
				</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/student/courses/{courseId}/assignments/{assignmentId}">
					{data.assignment.name}
				</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>Delivery</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/pass.png"
				alt="knowledge-sharing"
			/>
			<h1 class="ml-4 text-4xl font-semibold">Delivery</h1>
		</div>
	</div>
	<DeliveryFormCard
		assignment={data.assignment}
		assignmentFields={data.assignmentFields}
		delivery={data.delivery}
	/>
</div>
