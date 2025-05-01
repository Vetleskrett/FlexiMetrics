<script lang="ts">
	import { page } from '$app/stores';
	import TeacherAssignmentsCard from 'src/components/assignment/TeacherAssignmentsCard.svelte';
	import StudentsCard from 'src/components/student/StudentsCard.svelte';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import TeamsCard from 'src/components/team/TeamsCard.svelte';
	import TeachersCard from 'src/components/teacher/TeachersCard.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb';
	import type { Course, Assignment, Teacher, Team, CourseStudent } from 'src/types/';
	import { Role } from 'src/types/';
	import { goto } from '$app/navigation';
	import { deleteCourse } from 'src/api';
	import { handleErrors } from 'src/utils';
	import CustomAlertDialog from 'src/components/CustomAlertDialog.svelte';

	const courseId = $page.params.courseId;

	export let data: {
		course: Course;
		assignments: Assignment[];
		teachers: Teacher[];
		students: CourseStudent[];
		teams: Team[];
	};

	let showDelete = false;
	const onDeleteCoursePage = async () => {
		await handleErrors(async () => {
			await deleteCourse(courseId);
			goto('/teacher/courses');
		});
	};
</script>

<CustomAlertDialog
	bind:show={showDelete}
	description="This action cannot be undone. This will permanently delete the course."
	onConfirm={onDeleteCoursePage}
	action="Delete"
/>

<div class="m-auto mt-4 mb-16 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{data.course.code} - {data.course.name}</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/knowledge-sharing.png"
				alt="knowledge-sharing"
			/>
			<div>
				<h1 class="ml-4 text-4xl font-semibold">
					{data.course.code} - {data.course.name}
				</h1>
				<p class="ml-4 font-semibold text-gray-500">
					{data.course.year}
					{data.course.semester}
				</p>
			</div>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				<EllipsisVertical size={32} />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item href="{courseId}/edit">
					<Pencil class="h-4" />
					<p>Edit course</p>
				</DropdownMenu.Item>

				<DropdownMenu.Item on:click={() => (showDelete = true)}>
					<Trash2 class="h-4" />
					<p>Delete course</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</div>
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8">
			<TeacherAssignmentsCard assignments={data.assignments} {courseId} />
		</div>

		<div class="flex w-2/5 flex-col gap-8">
			<div class="flex flex-row gap-8">
				<StudentsCard numStudents={data.students.length} {courseId} />
				<TeamsCard numTeams={data.teams.length} {courseId} />
			</div>
			<TeachersCard {courseId} userRole={Role.Teacher} teachers={data.teachers} />
		</div>
	</div>
</div>
