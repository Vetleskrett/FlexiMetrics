<script lang="ts">
	import { page } from '$app/stores';
	import type { Team, Course, AssignmentProgress } from 'src/types/';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb';
	import CompletedTotalCard from 'src/components/CompletedTotalCard.svelte';
	import SimpleAddCard from 'src/components/SimpleAddCard.svelte';
	import TeacherTeamCard from 'src/components/team/TeacherTeamCard.svelte';
	import AssignmentsProgressCard from 'src/components/assignment/AssignmentsProgressCard.svelte';
	import { postStudentEmailTeam, deleteTeam } from 'src/api';
	import { goto } from '$app/navigation';
	import { handleErrors } from 'src/utils';
	import CustomAlertDialog from 'src/components/CustomAlertDialog.svelte';

	const courseId = $page.params.courseId;
	const teamId = $page.params.teamId;

	export let data: {
		course: Course;
		team: Team;
		assignmentsProgress: AssignmentProgress[];
	};

	const completed = data.assignmentsProgress.filter((item) => item.isDelivered).length;

	async function addStudent(input: string) {
		if (input && input.trim().length > 0) {
			await handleErrors(async () => {
				const result = await postStudentEmailTeam(data.team.id, {
					email: input
				});
				data.team = result.data;
			});
		}
	}

	let showDelete = false;
	const onDeleteTeam = async () => {
		await handleErrors(async () => {
			await deleteTeam(teamId);
			goto('./');
		});
	};
</script>

<CustomAlertDialog
	bind:show={showDelete}
	description="This action cannot be undone. This will permanently delete the team."
	onConfirm={onDeleteTeam}
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
				<Breadcrumb.Link href="/teacher/courses/{courseId}"
					>{data.course.code} - {data.course.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/teacher/courses/{courseId}/teams">Teams</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>Team {data.team.teamNr}</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img width="60" height="60" src="https://img.icons8.com/fluency/480/group.png" alt="group" />
			<div>
				<h1 class="ml-4 text-4xl font-semibold">Team {data.team.teamNr}</h1>
			</div>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				<EllipsisVertical size={32} />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item on:click={() => (showDelete = true)}>
					<Trash2 class="h-4" />
					<p>Delete Team</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</div>
	<div class="flex flex-row gap-8">
		<div class="flex w-[700px] flex-col gap-8">
			<TeacherTeamCard team={data.team} />
			<AssignmentsProgressCard assignmentsProgress={data.assignmentsProgress} />
		</div>
		<div class="flex w-[400px] flex-col gap-8">
			<SimpleAddCard
				headline="Add Member"
				actionString="Add"
				inputString="Email"
				inputType="String"
				addFunction={addStudent}
			/>
			<CompletedTotalCard
				{completed}
				total={data.assignmentsProgress.length}
				headline={'Delivered'}
			/>
		</div>
	</div>
</div>
