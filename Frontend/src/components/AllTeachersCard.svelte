<script lang="ts">
	import type { Teacher } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import * as Table from '$lib/components/ui/table';
	import CustomButton from './CustomButton.svelte';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import { removeTeacherFromCourse } from 'src/api';

	export let teachers: Teacher[], courseId: string;
	async function removeTeacher(teacher: Teacher) {
		try {
			await removeTeacherFromCourse(courseId, teacher.id);
			deleteTeacherFromList(teacher);
		} catch {
			console.error('Could not delete student');
		}
	}

	function deleteTeacherFromList(teacher: Teacher) {
		const index = teachers.indexOf(teacher, 0);
		if (index > -1) {
			teachers.splice(index, 1);
			teachers = teachers;
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
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each teachers as teacher}
					<Table.Row class="text-base ">
						<Table.Cell class="px-6">
							<p>{teacher.name}</p>
						</Table.Cell>
						<Table.Cell class="px-6">
							<p>{teacher.email}</p>
						</Table.Cell>
						<Table.Cell>
							<CustomButton color="red" on:click={() => removeTeacher(teacher)}>
								<Trash_2 />
							</CustomButton>
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
