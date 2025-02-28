<script lang="ts">
	import type { Student } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import CustomButton from './CustomButton.svelte';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import axios from 'axios';

	export let students: Student[], courseId: string;
	async function removeStudent(student: Student) {
		try{
			await axios.delete(`/api/course/${courseId}/students/${student.id}`)
			deleteStudentFromList(student)
		}
		catch{
			console.error("Could not delete student")
		}
	}

	function deleteStudentFromList(student: Student) {
		const index = students.indexOf(student, 0);
		if (index > -1) {
			students.splice(index, 1);
			students = students;
		}
	}
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Content class="p-0">
		<Table.Root class="h-full">
			<Table.Header>
				<Table.Row>
					<Table.Head class="h-8 px-6 font-bold text-black">Name</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Email</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Team</Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each students as student}
					<Table.Row class="text-base ">
						<Table.Cell class="px-6">
							<p>{student.name}</p>
						</Table.Cell>
						<Table.Cell class="px-6">
							<p>{student.email}</p>
						</Table.Cell>
						<Table.Cell class="px-6">
							<p>TBD</p>
						</Table.Cell>
						<Table.Cell>
							<CustomButton color="red" on:click={() => removeStudent(student)}>
								<Trash_2 />
							</CustomButton>
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
