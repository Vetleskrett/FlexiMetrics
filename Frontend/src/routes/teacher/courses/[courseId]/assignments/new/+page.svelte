<script lang="ts">
	import { postAssignment } from 'src/api';
	import { CollaborationType, type NewAssignmentField } from 'src/types';
	import { goto } from '$app/navigation';
	import CreateOrEditAssignment from 'src/components/CreateOrEditAssignment.svelte';
	import { page } from '$app/stores';

	const courseId = $page.params.courseId;

	async function addAssignment(assignmentName: string, dueDate: string, collaborationType: CollaborationType, draft: boolean, fields: NewAssignmentField[]) {
		try {
			await postAssignment({
				name: assignmentName,
				dueDate: dueDate,
				published: !draft,
				collaborationType: collaborationType == CollaborationType.Individual ? 0 : 1,
				courseId: courseId,
				fields: fields
			})
			goto(`/teacher/courses/${courseId}`)
		}
		catch(exception){
			console.error("Something Went Wrong!")
		}
	}
</script>

<CreateOrEditAssignment submitFunction={addAssignment} redirect={`/teacher/courses/${courseId}`}/>
