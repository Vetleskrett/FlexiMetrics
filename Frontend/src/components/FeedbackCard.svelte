<script lang="ts">
	import { type Assignment, type Feedback, type GradingType } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Check from 'lucide-svelte/icons/check';
	import X from 'lucide-svelte/icons/x';

	export let assignment: Assignment;
	export let feedback: Feedback | null;

	const getGradingString = (gradingType: GradingType, feedback: Feedback) => {
		switch (gradingType) {
			case 'ApprovalGrading':
				return feedback!.isApproved! ? 'Approved' : 'Not approved';

			case 'LetterGrading':
				return feedback!.letterGrade;

			case 'PointsGrading':
				return feedback?.points?.toString() + ' / ' + assignment.maxPoints!.toString();
		}
		return null;
	};
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="40"
				height="40"
				src="https://img.icons8.com/fluency/480/popular-topic--v1.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-2 text-2xl">Feedback</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#if feedback}
				{#if assignment?.gradingType != 'NoGrading'}
					<Separator class="w-full" />
					<div class="flex items-center justify-between px-6 py-4">
						<h1 class="font-semibold">Grading</h1>
						{#if assignment.gradingType == 'ApprovalGrading'}
							<div class="flex h-full items-center justify-center gap-1">
								{#if feedback.isApproved}
									<Check color="green" />
									<p>Approved</p>
								{:else}
									<X color="red" />
									<p>Disapproved</p>
								{/if}
							</div>
						{:else if assignment.gradingType == 'LetterGrading'}
							<div class="text-center text-lg">
								<p>{feedback.letterGrade}</p>
							</div>
						{:else if assignment.gradingType == 'PointsGrading'}
							<div class="whitespace-nowrap text-lg">
								<p>{feedback.points} / {assignment.maxPoints}</p>
							</div>
						{/if}
					</div>
				{/if}
				<Separator class="w-full" />
				<div class="px-6 py-4">
					<h1 class="font-semibold">Comment</h1>
					<h1>{feedback.comment}</h1>
				</div>
			{:else}
				<p class="mx-auto py-4">No feedback received</p>
			{/if}
		</div>
	</Card.Content>
</Card.Root>
