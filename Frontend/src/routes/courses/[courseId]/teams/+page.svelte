<script lang="ts">
	import { page } from '$app/stores';
	import AllTeamCard from 'src/components/AllTeamCard.svelte';
	import type { Team, Student } from 'src/types';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import AddTeamMembersCard from 'src/components/AddTeamMembersCard.svelte';
	import SimpleAddCard from 'src/components/SimpleAddCard.svelte';
	import TeamAssignmentsCard from 'src/components/TeamAssignmentsCard.svelte';

	const courseId = $page.params.courseId;

	const course = {
		id: '1',
		code: 'TDT101',
		name: 'Programmering',
		year: 2024,
		semester: 'Autumn'
	};
	const students: Student[] = [
		{
			id: 'abc',
			name: 'Ola Nordmann',
			email: 'OlaNordmann@ntnu.no'
		},
		{
			id: 'abc',
			name: 'Ola Nordmann',
			email: 'OlaNordmann@ntnu.no'
		},
		{
			id: 'abc',
			name: 'Ola Nordmann',
			email: 'OlaNordmann@ntnu.no'
		}
	];

	const teams: Team[] = [];
	for (let i = 1; i <= 10; i++) {
		teams.push({
			id: i.toString(),
			students: students,
			complete: Math.floor(Math.random() * 100)
		});
	}
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/courses/{courseId}">{course.code} - {course.name}</Breadcrumb.Link>
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
			<AllTeamCard {teams} {courseId} />
		</div>
		<div class="flex w-[400px] flex-col gap-8">
			<SimpleAddCard
				headline="Add Teams"
				actionString="Add"
				inputString="Number of Teams"
				inputType="Number"
			/>
			<AddTeamMembersCard />
		</div>
	</div>
</div>
