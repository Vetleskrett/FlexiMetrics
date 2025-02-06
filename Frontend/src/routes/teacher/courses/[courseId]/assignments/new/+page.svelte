<script lang="ts">
	import { postAssignment } from 'src/api';
	import { CollaborationType, GradingTypeEnum, type NewAssignmentField } from 'src/types';
	import { goto } from '$app/navigation';
	import CreateOrEditAssignment from 'src/components/CreateOrEditAssignment.svelte';
	import { page } from '$app/stores';

	const courseId = $page.params.courseId;

	async function addAssignment(
		assignmentName: string,
		dueDate: string,
		collaborationType: CollaborationType,
		draft: boolean,
		mandatory: boolean,
		fields: NewAssignmentField[],
		description: string,
		gradingType: GradingTypeEnum,
		maxPoints: number
	) {
		try {
			await postAssignment({
				name: assignmentName,
				description: description,
				dueDate: dueDate,
				mandatory: mandatory,
				published: !draft,
				gradingType: gradingType,
				maxPoints: maxPoints,
				collaborationType: collaborationType,
				courseId: courseId,
				fields: fields
			});
			goto(`/teacher/courses/${courseId}`);
		} catch (exception) {
			console.error('Something Went Wrong!');
		}
	}
</script>

<CreateOrEditAssignment submitFunction={addAssignment} redirect={`/teacher/courses/${courseId}`} />
