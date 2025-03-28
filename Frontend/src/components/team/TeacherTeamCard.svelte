<script lang="ts">
	import type { Student, Team } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import Ellipsis from 'lucide-svelte/icons/ellipsis';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import { deleteStudentTeam } from 'src/api';
	import * as Table from '$lib/components/ui/table';

	export let team: Team;

	let students = team.students;

	function deleteStudentFromList(student: Student) {
		const index = students.indexOf(student, 0);
		if (index > -1) {
			students.splice(index, 1);
			students = students;
		}
	}

	async function removeTeamMember(student: Student) {
		try {
			await deleteStudentTeam(team.id, student.id);
			deleteStudentFromList(student);
		} catch {
			console.error('Could not remove team member');
		}
	}
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/student-male.png"
				alt="student-icon"
			/>
			<Card.Title class="ml-4 text-3xl">Students</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<Table.Root>
			<Table.Header>
				<Table.Row>
					<Table.Head class="h-0 px-6 font-bold text-black">Name</Table.Head>
					<Table.Head class="h-0 px-6 font-bold text-black">Email</Table.Head>
					<Table.Head class="h-0 px-6 font-bold text-black"></Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each students as student}
					<Table.Row>
						<Table.Cell class="px-6">
							{student.name}
						</Table.Cell>
						<Table.Cell class="px-6">
							{student.email}
						</Table.Cell>
						<Table.Cell class="px-6">
							<DropdownMenu.Root>
								<DropdownMenu.Trigger>
									<Ellipsis />
								</DropdownMenu.Trigger>
								<DropdownMenu.Content>
									<DropdownMenu.Item on:click={() => removeTeamMember(student)}>
										<Trash2 class="h-4" />
										<p>Remove member</p>
									</DropdownMenu.Item>
								</DropdownMenu.Content>
							</DropdownMenu.Root>
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
