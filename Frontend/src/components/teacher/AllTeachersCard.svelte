<script lang="ts">
	import type { Teacher } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import CustomButton from '../CustomButton.svelte';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import { removeTeacherFromCourse } from 'src/api';
	import { handleErrors } from 'src/utils';
	import CustomAlertDialog from '../CustomAlertDialog.svelte';

	export let teachers: Teacher[], courseId: string;

	let showRemove = false;
	let teacherToRemove: Teacher | undefined = undefined;
	async function removeTeacher() {
		await handleErrors(async () => {
			await removeTeacherFromCourse(courseId, teacherToRemove!.id);
			deleteTeacherFromList(teacherToRemove!);
		});
	}

	function deleteTeacherFromList(teacher: Teacher) {
		const index = teachers.indexOf(teacher, 0);
		if (index > -1) {
			teachers.splice(index, 1);
			teachers = teachers;
		}
	}
</script>

<CustomAlertDialog
	bind:show={showRemove}
	description={`This will remove ${teacherToRemove?.name} from the course.`}
	onConfirm={removeTeacher}
	action="Remove"
/>

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
							<CustomButton
								color="red"
								outline={true}
								on:click={() => {
									teacherToRemove = teacher;
									showRemove = true;
								}}
							>
								<Trash_2 size="16" />
							</CustomButton>
						</Table.Cell>
					</Table.Row>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
