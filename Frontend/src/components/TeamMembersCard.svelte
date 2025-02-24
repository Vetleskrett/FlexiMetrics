<script lang="ts">
	import type { Student } from 'src/types.ts';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Ellipsis from 'lucide-svelte/icons/ellipsis';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import { deleteStudentTeam } from 'src/api';

	export let students: Student[], teamId: string;

	//TODO: Maybe turn this into a helper function or find a better solution for updating the list
	function deleteStudentFromList(student: Student) {
		const index = students.indexOf(student, 0);
		if (index > -1) {
			students.splice(index, 1);
			students = students;
		}
	}

	async function removeTeamMember(student: Student) {
		try {
			await deleteStudentTeam(teamId, student.id);
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
		<div class="flex flex-col">
			<div class="flex items-center px-6 text-sm font-bold">
				<h1 class="w-2/3">Name</h1>
				<h1 class="w-2/3">Mail</h1>
				<div class="w-1/4"></div>
			</div>
			{#each students as student}
				<Separator class="w-full" />
				<div class="flex items-center px-6 py-4">
					<h1 class="w-2/3">{student.name}</h1>
					<h1 class="w-2/3">{student.email}</h1>
					<div class="w-1/4">
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
					</div>
				</div>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
