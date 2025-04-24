<script lang="ts">
	import { page } from '$app/stores';
	import type { Student, Course, AssignmentProgress, Team } from 'src/types/';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb';
	import CompletedTotalCard from 'src/components/CompletedTotalCard.svelte';
	import AssignmentsProgressCard from 'src/components/assignment/AssignmentsProgressCard.svelte';
	import { deleteStudentCourse } from 'src/api';
	import { goto } from '$app/navigation';
	import StudentTeamCard from 'src/components/team/StudentTeamCard.svelte';
	import { handleErrors } from 'src/utils';
	import CustomAlertDialog from 'src/components/CustomAlertDialog.svelte';

	const courseId = $page.params.courseId;
	const studentId = $page.params.studentId;

	export let data: {
		course: Course;
		student: Student;
		assignmentsProgress: AssignmentProgress[];
		team?: Team;
	};

	const completed = data.assignmentsProgress.filter((item) => item.isDelivered).length;

	let showRemove = false;
	const onRemoveStudent = async () => {
		await handleErrors(async () => {
			await deleteStudentCourse(courseId, studentId);
			goto('./');
		});
	};
</script>

<CustomAlertDialog
	bind:show={showRemove}
	description={`This will remove ${data.student.name} from the course.`}
	onConfirm={onRemoveStudent}
	action="Remove"
/>

<div class="m-auto mt-4 mb-16 flex w-max flex-col items-center justify-center gap-10">
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
				<Breadcrumb.Link href="/teacher/courses/{courseId}/students">Students</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{data.student.name}</Breadcrumb.Page>
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
				<h1 class="ml-4 text-4xl font-semibold">
					{data.student.name}
				</h1>
				<p class="ml-4 font-semibold text-gray-500">
					{data.student.email}
				</p>
			</div>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				<EllipsisVertical size={32} />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item on:click={() => (showRemove = true)}>
					<Trash2 class="h-4" />
					<p>Remove Student</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</div>
	<div class="flex flex-row gap-8">
		<div class="flex w-[700px] flex-col gap-8">
			<AssignmentsProgressCard assignmentsProgress={data.assignmentsProgress} />
		</div>
		<div class="flex w-[400px] flex-col gap-8">
			<StudentTeamCard team={data.team} />
			<CompletedTotalCard
				{completed}
				total={data.assignmentsProgress.length}
				headline={'Delivered'}
			/>
		</div>
	</div>
</div>
