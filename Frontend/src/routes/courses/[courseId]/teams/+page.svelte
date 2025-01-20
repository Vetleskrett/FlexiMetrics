<script lang="ts">
	import { page } from '$app/stores';
	import AllTeamCard from 'src/components/AllTeamCard.svelte';
	import type { Team, Course } from 'src/types';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import AddTeamMembersCard from 'src/components/AddTeamMembersCard.svelte';
	import SimpleAddCard from 'src/components/SimpleAddCard.svelte';
	import { getCourse, getTeams, postTeams } from 'src/api';
	import { onMount } from 'svelte';

	const courseId = $page.params.courseId;

	let course : Course;
	let teams : Team[] = []

	onMount(async () => {
		try{
			course = await getCourse(courseId);
			teams = await getTeams(courseId);
		}
		catch(error){
			console.error("Something went wrong!")
		}
	})
	async function addTeams(input: number){
		if (input && input > 0){
			try{
				await postTeams(
					{
						courseId: courseId,
						numTeams: input,
					});
				teams = await getTeams(courseId);
				}
			catch(error){
				console.error("Something went wrong!")
			}
		}
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
				<Breadcrumb.Link href="/courses/{courseId}">{course?.code} - {course?.name}</Breadcrumb.Link>
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
				addFunction={addTeams}
			/>
			<AddTeamMembersCard />
		</div>
	</div>
</div>
