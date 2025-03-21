<script lang="ts">
	import { page } from '$app/stores';
	import AllTeamCard from 'src/components/team/AllTeamCard.svelte';
	import type { Team, Course, StudentToTeam } from 'src/types/';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb';
	import AddTeamMembersCard from 'src/components/team/AddTeamMembersCard.svelte';
	import SimpleAddCard from 'src/components/SimpleAddCard.svelte';
	import { postStudentsTeam, postTeams } from 'src/api';

	const courseId = $page.params.courseId;

	export let data: {
		course: Course;
		teams: Team[];
	};

	async function addTeams(input: number) {
		if (input && input > 0) {
			try {
				const result = await postTeams({
					courseId: courseId,
					numTeams: input
				});
				data.teams = result.data;
			} catch (error) {
				console.error('Something went wrong!');
			}
		}
	}

	async function addTeamMembers(input: string, file: File | null) {
		if (file) {
			input = await file.text();
		}
		const allTeams: StudentToTeam[] = [];
		const newTeams = input.split('\n');
		for (const team of newTeams) {
			const rawInfo = team.split(',');
			if (!checkInfo(rawInfo)) {
				console.warn('Something is wrong with the input');
				return;
			}
			const info = rawInfo.map((i) => i.trim());
			allTeams.push({ teamNr: Number(info[0].trim()), emails: info.slice(1) });
		}
		try {
			const result = await postStudentsTeam({
				courseId: courseId,
				teams: allTeams
			});
			data.teams = result.data;
		} catch (error) {
			console.error('Something went wrong!');
		}
	}

	function checkInfo(info: string[]): boolean {
		// Add additional checks if needed
		if (info.length < 2 || isNaN(Number(info[0].trim()))) {
			return false;
		}
		for (const i of info.slice(1)) {
			if (i.trim().length < 1) {
				return false;
			}
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
				<Breadcrumb.Page>Teams</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img width="60" height="60" src="https://img.icons8.com/fluency/480/group.png" alt="group" />
			<div>
				<h1 class="ml-4 text-4xl font-semibold">Teams</h1>
			</div>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				<EllipsisVertical size={32} />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item>
					<Trash2 class="h-4" />
					<p>Delete All Teams</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</div>
	<div class="flex flex-row gap-8">
		<div class="flex w-[700px] flex-col gap-8">
			<AllTeamCard teams={data.teams} {courseId} />
		</div>
		<div class="flex w-[400px] flex-col gap-8">
			<SimpleAddCard
				headline="Add Teams"
				actionString="Add"
				inputString="Number of Teams"
				inputType="Number"
				addFunction={addTeams}
			/>
			<AddTeamMembersCard addFunction={addTeamMembers} />
		</div>
	</div>
</div>
