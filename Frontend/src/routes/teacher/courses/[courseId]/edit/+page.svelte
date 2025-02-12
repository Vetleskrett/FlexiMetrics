<script lang="ts">
	import CreateOrEditCourse from 'src/components/CreateOrEditCourse.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import type { Course } from 'src/types';

	export let data: {
		course: Course;
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
				<Breadcrumb.Page>Edit Course</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<CreateOrEditCourse edit={true} course={data.course} />
</div>
