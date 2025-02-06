<script lang="ts">
	import { editAssignment } from 'src/api';
	import {
		CollaborationType,
		GradingTypeEnum,
		type Assignment,
		type NewAssignmentField
	} from 'src/types';
	import { goto } from '$app/navigation';
	import CreateOrEditAssignment from 'src/components/CreateOrEditAssignment.svelte';
	import { page } from '$app/stores';

	const courseId = $page.params.courseId;
	const assignmentId = $page.params.assignmentId;

	export let data: {
		assignment: Assignment;
	};

	async function editAssignments(
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
			await editAssignment(assignmentId, {
				name: assignmentName,
				description: description,
				dueDate: dueDate,
				mandatory: mandatory,
				published: !draft,
				gradingType: gradingType,
				maxPoints: maxPoints,
				collaborationType: collaborationType,
				courseId: courseId
			});
			goto(`/teacher/courses/${courseId}/assignments/${assignmentId}`);
		} catch (exception) {
			console.error('Something Went Wrong!');
		}
	}
</script>

<CreateOrEditAssignment
	asssignmentName={data.assignment.name}
	description={data.assignment.description}
	mandatory={data.assignment.mandatory}
	collaberationType={CollaborationType[data.assignment.collaborationType]}
	gradingType={GradingTypeEnum[data.assignment.gradingType]}
	dueDate={data.assignment.dueDate}
	maxPoints={data.assignment.maxPoints ? data.assignment.maxPoints : 0}
	draft={!data.assignment.published}
	submitFunction={editAssignments}
	edit={true}
	redirect={`/teacher/courses/${courseId}/assignments/${assignmentId}`}
/>
