<script lang="ts">
	import { page } from '$app/stores';
	import type { Team, Student, AssignmentTeam, Course, StudentAssignment } from 'src/types';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import CompletedTotalCard from 'src/components/CompletedTotalCard.svelte';
	import SimpleAddCard from 'src/components/SimpleAddCard.svelte';
	import TeamMembersCard from 'src/components/TeamMembersCard.svelte';
	import TeamAssignmentsCard from 'src/components/TeamAssignmentsCard.svelte';
	import { postStudentEmailTeam, getTeam } from 'src/api';

	const courseId = $page.params.courseId;

	export let data: {
		course: Course
		team: Team;
		assignments: StudentAssignment[]
	};

	let completed = data.assignments.filter(item => item.isDelivered).length

	async function addStudent(input: string) {
		if (input && input.trim().length > 0) {
			try {
				const result = await postStudentEmailTeam(data.team.id, {
					email: input
				});
				data.team = result.data;
			} catch (error) {
				console.error('Could not add student!');
			}
		}
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
				<DropdownMenu.Item>
					<Trash2 class="h-4" />
					<p>Delete Team</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</div>
	<div class="flex flex-row gap-8">
		<div class="flex w-[700px] flex-col gap-8">
			<TeamMembersCard students={data.team.students} teamId={data.team.id}/>
			<TeamAssignmentsCard assignmentsTeam={data.assignments} />
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
				completed={completed}
				total={data.assignments.length}
				headline={'Assignments Delivered'}
			/>
		</div>
	</div>
</div>
